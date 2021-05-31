using System.Threading;
using System.Threading.Tasks;
using DAM2.Core.Shared.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Client1
{
	internal class LifetimeEventsHostedService : IHostedService
	{
		private readonly ILogger<LifetimeEventsHostedService> logger;
		private readonly IHostApplicationLifetime appLifetime;
		private readonly ISharedClusterClient sharedClusterWorker;

		public LifetimeEventsHostedService(ILogger<LifetimeEventsHostedService> logger,
			IHostApplicationLifetime appLifetime,
			ISharedClusterClient sharedClusterWorker)
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

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await this.sharedClusterWorker.Shutdown();
			Log.CloseAndFlush();
		}

		private void OnStopped()
		{
			logger.LogInformation("OnStopped has been called.");
		}

		private void OnStopping()
		{
			logger.LogInformation("OnStopping has been called.");
		}

		private void OnStarted()
		{
			logger.LogInformation("OnStarted has been called.");
		}
	}
}
