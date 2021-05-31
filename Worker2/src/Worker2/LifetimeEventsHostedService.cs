using System.Threading;
using System.Threading.Tasks;
using DAM2.Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Worker2
{

	public class ForceShutdown
	{
		public static IHostApplicationLifetime appLifetime;

	}
	internal class LifetimeEventsHostedService : IHostedService
	{
		private readonly ILogger<LifetimeEventsHostedService> logger;
		private readonly IHostApplicationLifetime appLifetime;
		private readonly SharedClusterWorker sharedClusterWorker;

		public LifetimeEventsHostedService(ILogger<LifetimeEventsHostedService> logger,
			IHostApplicationLifetime appLifetime,
			SharedClusterWorker sharedClusterWorker)
		{
			this.logger = logger;
			this.appLifetime = appLifetime;
			this.sharedClusterWorker = sharedClusterWorker;
		}
		public Task StartAsync(CancellationToken cancellationToken)
		{
			appLifetime.ApplicationStarted.Register(OnStarted);
			appLifetime.ApplicationStopping.Register(OnStopping);
			appLifetime.ApplicationStopped.Register(OnStopped);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("StopAsync has been called.");
			Log.CloseAndFlush();
			return Task.CompletedTask;
		}

		private void OnStopped()
		{
			logger.LogInformation("OnStopped has been called.");
		}

		private void OnStopping()
		{
			this.sharedClusterWorker.Shutdown().GetAwaiter().GetResult();
			logger.LogInformation("OnStopping has been called.");
		}

		private void OnStarted()
		{
			this.sharedClusterWorker.Run().ConfigureAwait(false).GetAwaiter().GetResult();
			logger.LogInformation("OnStarted has been called.");
		}
	}
}
