using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Serilog;
using Serilog.Events;
using Log = Serilog.Log;
using DAM2.Core.Shared;
using DAM2.Core.Shared.Interface;
using DAM2.Core.Shared.Settings;
using DAM2.Shared.Shared.Interface;
using DAM2.Shared.Shared;

namespace Worker2
{
	
	class Program
	{

		static int Main(string[] args)
		{
			try
			{
				return MainAsync(args).Result;
			}
			catch
			{
				return 1;
			}
		}


		static async Task<int> MainAsync(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.MinimumLevel.Information()
				.MinimumLevel.Override("System", LogEventLevel.Information)
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.WriteTo.Console()
				.CreateLogger();

			try
			{
				await CreateHostBuilder(args).Build().RunAsync().ConfigureAwait(false);
				return 0;
			}
			catch (Exception ex)
			{
				Log.Logger.Fatal(ex, "Failed application");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureHostConfiguration((configHost) =>
				{
					configHost.SetBasePath(Directory.GetCurrentDirectory());
					configHost.AddJsonFile("appsettings.json", optional: true);
					configHost.AddCommandLine(args);
					configHost.AddEnvironmentVariables();
				})
				.ConfigureAppConfiguration((ctx, builder) =>
				{
					builder.AddUserSecrets<Program>(true);
					var config = builder.Build();
					string debugView = config.GetDebugView();
					Log.Logger.Information("{Configuration}", debugView);

				})
				.ConfigureLogging((context, logging) => { logging.ClearProviders(); })
				.ConfigureServices((hostContext, services) =>
				{
					var configuration = hostContext.Configuration;

					services.AddCustomSerilog(hostContext.Configuration, hostContext.HostingEnvironment, "Actors");

					services.Scan(scan =>
						scan.FromAssemblies(typeof(Program).Assembly)
							.AddClasses(classes => classes.AssignableTo<IActor>())
							.AsSelf()
							.WithTransientLifetime()
					);

					// configure ClusterSettings
					services.Configure<ClusterSettings>(configuration.GetSection("ClusterSettings"));

					// add IGenericDataBaseSettings
					services.AddSingleton<IGenericDataBaseSettings>(sp =>
					{
						var firstSetting = sp.GetRequiredService<IOptions<GenericDataBaseSettings>>();
						return firstSetting.Value;
					});

					// add IClusterSettings
					services.AddSingleton<IClusterSettings>(sp =>
					{
						var firstSetting = sp.GetRequiredService<IOptions<ClusterSettings>>();
						return firstSetting.Value;
					});

					// add SharedClusterProvider
					services.AddSingleton<ISharedClusterProviderFactory, SharedClusterProviderFactory>();

					// add SetupRootActors
					services.AddTransient<ISharedSetupRootActors, SetupRootActors>();

					services.AddTransient<ITokenFactory, TokenFactory>();

					// add DescriptorProvider
					services.AddScoped<IDescriptorProvider, DescriptorProvider>();

					// add MainWorker
					services.AddScoped<IMainWorker, MainWorker>();

					// add ClusterWorker
					services.AddSingleton<SharedClusterWorker>();

					services.AddHostedService<LifetimeEventsHostedService>();
				});

	}
}
