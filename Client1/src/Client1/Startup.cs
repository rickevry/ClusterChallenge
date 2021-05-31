using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using DAM2.Core.Shared;
using DAM2.Core.Shared.Interface;
using DAM2.Core.Shared.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Client1
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection serviceCollection)
        {

            serviceCollection.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            serviceCollection.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            serviceCollection.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            serviceCollection.AddLogging();

            serviceCollection.Configure<ClusterSettings>(Configuration.GetSection("ClusterSettings"));

            serviceCollection.AddSingleton<IClusterSettings>(sp => {
                var firstSetting = sp.GetRequiredService<IOptions<ClusterSettings>>();
                return firstSetting.Value;
            });

            // add SharedClusterProvider
            serviceCollection.AddSingleton<ISharedClusterProviderFactory, SharedClusterProviderFactory>();

            // add DescriptorProvider
            serviceCollection.AddSingleton<IDescriptorProvider, DescriptorProvider>();

            // add SharedClusterClient
            serviceCollection.AddSingleton<ISharedClusterClient, SharedClusterClient>();

            serviceCollection.AddControllers().AddNewtonsoftJson();

            serviceCollection.AddHostedService<LifetimeEventsHostedService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

