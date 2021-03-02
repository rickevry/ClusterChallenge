using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;

namespace Worker2
{
    public static class LoggingRegistry
    {
        public static IServiceCollection AddCustomSerilog(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment,
            string serviceName = default
        )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            AddApplicationInsights(configuration, services);
            AddDiagnosticContext(services);
            AddLoggerFactory(services, configuration, environment, serviceName);

            return services;
        }

        private static void AddLoggerFactory(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment, string serviceName = default)
        {
            services.AddSingleton(provider => {
                var loggerConfiguration = new LoggerConfiguration();

                loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Verbose()
                    //.MinimumLevel.Information()
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .WriteTo.Console(); 

                if (!string.IsNullOrWhiteSpace(serviceName))
                {
                    loggerConfiguration.Enrich.WithProperty("Service", serviceName);
                }

                AddTelemetryConfiguration(configuration, services, provider, hostEnvironment, loggerConfiguration);

                Logger logger = loggerConfiguration.CreateLogger();
                Log.Logger = logger;
                ILoggerFactory factory = new Serilog.Extensions.Logging.SerilogLoggerFactory(logger, true, null);
                Proto.Log.SetLoggerFactory(factory);
                return factory;
            });
        }

        private static void AddTelemetryConfiguration(IConfiguration configuration, IServiceCollection services, IServiceProvider serviceProvider, IHostEnvironment environment, LoggerConfiguration loggerConfiguration)
        {
            string aiInstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];

            if (string.IsNullOrWhiteSpace(aiInstrumentationKey))
            {
                return;
            }

            AddTemporaryFolder(services, environment);

            TelemetryConfiguration telemetryConfiguration = serviceProvider.GetRequiredService<TelemetryConfiguration>();
            loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
        }
        private static void AddDiagnosticContext(IServiceCollection services)
        {
            services.AddSingleton(new DiagnosticContext(Log.Logger));
            // Consumed by e.g. middleware
            services.AddSingleton<IDiagnosticContext>((cxt) => {
                DiagnosticContext diagnosticContext = cxt.GetRequiredService<DiagnosticContext>();
                return diagnosticContext;
            });
        }

        private static void AddApplicationInsights(IConfiguration configuration, IServiceCollection services)
        {
            string aiInstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];

            if (string.IsNullOrWhiteSpace(aiInstrumentationKey))
            {
                return;
            }

            services.AddApplicationInsightsTelemetryWorkerService(configuration);

            string roleName = configuration["ApplicationInsights:ServiceRoleName"];
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameTelemetryInitializer(roleName));
            }

            
        }

        private static void AddTemporaryFolder(IServiceCollection services, IHostEnvironment environment)
        {
            if (environment == null)
            {
                return;
            }

            if (environment.IsProduction() && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DirectoryInfo tempDirectory = new DirectoryInfo(Path.Combine(environment.ContentRootPath, "appInsightsTemp"));
                if (!tempDirectory.Exists)
                {
                    tempDirectory.Create();
                }

                services.AddSingleton(typeof(ITelemetryChannel), new ServerTelemetryChannel() { StorageFolder = tempDirectory.FullName });
            }
        }
    }
}
