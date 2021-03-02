using System.Threading;
using System.Threading.Tasks;
using DAM2.Core.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Worker1
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
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			appLifetime.ApplicationStarted.Register(OnStarted);
			appLifetime.ApplicationStopping.Register(OnStopping);
			appLifetime.ApplicationStopped.Register(OnStopped);

			await this.sharedClusterWorker.Run().ConfigureAwait(false);
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
