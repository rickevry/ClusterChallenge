using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Remote.GrpcCore;
using DAM2.Core.Shared.Interface;
using DAM2.Core.Shared.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DAM2.Core.Shared
{
	public class SharedClusterClient : ISharedClusterClient
	{
		private readonly ILogger<SharedClusterClient> _logger;
		private readonly IDescriptorProvider _descriptorProvider;
		private readonly IClusterSettings _clusterSettings;
		private Cluster _cluster;
		private bool _cluster_ready = false;
		private readonly ISharedClusterProviderFactory _clusterProvider;
		private Dictionary<string, int> retries = new();
		private CancellationTokenSource connectedTokenSource = new CancellationTokenSource();

		public SharedClusterClient(ILogger<SharedClusterClient> logger, IDescriptorProvider descriptorProvider, IClusterSettings clusterSettings, ISharedClusterProviderFactory clusterProvider)
		{
			_logger = logger;
			_descriptorProvider = descriptorProvider;
			_clusterSettings = clusterSettings;
			_clusterProvider = clusterProvider;
		}

		public bool Connected { get; private set; }

		public async Task Startup()
		{
			await CreateCluster();
		}

		public async Task Shutdown()
		{
			this.connectedTokenSource.Cancel(false);
			if (this._cluster != null)
			{
				await this._cluster.ShutdownAsync(true).ConfigureAwait(false);
				await Task.Delay(1000);
			}
		}

		public async Task<Cluster> CreateCluster()
		{
			try
			{
				_logger.LogInformation("Setting up Cluster without actors");
				_logger.LogInformation("ClusterName: " + _clusterSettings.ClusterName);
				_logger.LogInformation("PIDDatabaseName: " + _clusterSettings.PIDDatabaseName);
				_logger.LogInformation("PIDCollectionName: " + _clusterSettings.PIDCollectionName);

				var system = new ActorSystem();
				var clusterProvider = _clusterProvider.CreateClusterProvider(_logger);

				var identity = MongoIdentityLookup.GetIdentityLookup(_clusterSettings.ClusterName, _clusterSettings.PIDConnectionString, _clusterSettings.PIDCollectionName, _clusterSettings.PIDDatabaseName);
				//var identity = RedisIdentityLookup.GetIdentityLookup(_clusterSettings.ClusterName, _clusterSettings.Host, _clusterSettings.RedisPort);

				var (clusterConfig, remoteConfig) = GenericClusterConfig.CreateClusterConfig(_clusterSettings, clusterProvider, identity, _descriptorProvider, _logger);

				//clusterConfig = _setupRootActors.AddRootActors(clusterConfig);

				_ = new GrpcCoreRemote(system, remoteConfig);
				var cluster = new Cluster(system, clusterConfig);

				await cluster.StartClientAsync().ConfigureAwait(false);

				_ = SafeTask.Run(async () =>
				{
					int counter = 0;
					while (!connectedTokenSource.Token.IsCancellationRequested)
					{
						try
						{
							Member[] members = _cluster.MemberList.GetAllMembers();

							this.Connected = members.Length > 0;

							if (!this.Connected)
							{
								counter = 0;
								_logger.LogInformation("[SharedClusterClient] Connected {Connected}", this.Connected);
							}

							if (this.Connected)
							{
								if (counter % 20 == 0 || counter == 0)
								{
									_logger.LogDebug("[SharedClusterClient] Members {@Members}",
										members.Select(m => m.ToLogString()));
								}
								counter++;
							}

							await Task.Delay(500);
						}
						catch
						{
							// ignored
						}
					}
				}, connectedTokenSource.Token);


				_cluster = cluster;
				_cluster_ready = true;

				_logger.LogInformation("Cluster Client ready");

				return cluster;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SharedClusterClient failed");
				return null;
			}
		}

		public Cluster Cluster => _cluster;

		public async Task<T> RequestAsync<T>(string actorPath, string clusterKind, object cmd)
		{
			string key = $"{actorPath}_{clusterKind}";
			int counter = 0;
			while (!_cluster_ready && counter < 40)
			{
				await Task.Delay(250).ConfigureAwait(false);
				counter++;
			}

			try
			{
				var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
				var res = await _cluster.RequestAsync<T>(actorPath, clusterKind, cmd, tokenSource.Token).ConfigureAwait(false);

				if (tokenSource.Token.IsCancellationRequested)
				{
					_cluster_ready = false;
					await RestartMe();
					return await Retry<T>(actorPath, clusterKind, cmd, key);
				}
				retries.Remove(key);

				return res;
			}
			catch (TimeoutException)
			{
				return await Retry<T>(actorPath, clusterKind, cmd, key);
			}
			catch (Exception x)
			{
				_logger.LogError(x, "Failed Request {Id}", actorPath);
				return default(T);
			}
		}

		private async Task<T> Retry<T>(string actorPath, string clusterKind, object cmd, string key)
		{
			if (retries.TryGetValue(key, out int value))
			{
				Interlocked.Increment(ref value);
			}
			else
			{
				retries.Add(key, 1);
			}

			if (value > 5)
			{
				_logger.LogError("Request timeout for {Id}", actorPath);
				return default(T);
			}

			_logger.LogInformation("Retry request...");
			await Task.Delay(value * 200);
			return await RequestAsync<T>(actorPath, clusterKind, cmd);
		}

		private Task RestartMe()
		{
			_ = SafeTask.Run(async () =>
			{
				try { await this.Shutdown(); }
				catch
				{
					// ignored
				}
				await Task.Delay(5000);
				this._cluster = null;
				this.connectedTokenSource = new CancellationTokenSource();
				await this.CreateCluster();
				await Task.Delay(5000);
			});

			return Task.CompletedTask;
		}
	}
}
