using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Client1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
				 .ConfigureHostConfiguration((configHost) =>
				 {
					 configHost.SetBasePath(Directory.GetCurrentDirectory());
					 configHost.AddJsonFile("secrets/appsettings.json", optional: true);
					 configHost.AddCommandLine(args);
					 configHost.AddEnvironmentVariables();
				 })
				 .ConfigureAppConfiguration((ctx, builder) =>
				 {
					 if (ctx.HostingEnvironment.IsDevelopment())
					 {
						 builder.AddJsonFile("appsettings.json", optional: true);
						 builder.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", true);
						 builder.AddUserSecrets<Program>(true);
					 }

					 builder.AddJsonFile("secrets/appsettings.json", optional: true);

					 if (!ctx.HostingEnvironment.IsProduction())
					 {
						 var config = builder.Build();
						 string debugView = config.GetDebugView();
						 Log.Logger.Information("{Configuration}", debugView);
					 }
					
				 })
				 .ConfigureLogging((context, logging) => { logging.ClearProviders(); })
				 .ConfigureServices((context, services) =>
				 {
					 services.AddCustomSerilog(context.Configuration, context.HostingEnvironment, "Rest");
				 })
				 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
    }
}
