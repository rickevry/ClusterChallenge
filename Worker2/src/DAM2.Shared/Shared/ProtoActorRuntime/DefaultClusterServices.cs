using DAM2.Core.Shared;
using DAM2.Core.Shared.Interface;
using DAM2.Core.Shared.Settings;
using DAM2.Shared.Shared.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM2.Shared.Shared.ProtoActorRuntime
{
    internal static class DefaultClusterServices
    {
        internal static void AddDefaultServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton<ISharedClusterProviderFactory, SharedClusterProviderFactory>();
            services.AddTransient<ITokenFactory, TokenFactory>();

            services.AddSingleton<IClusterSettings>(_ => _.GetRequiredService<IOptions<ClusterSettings>>().Value);

            services.AddSingleton<SharedClusterWorker>();
        }
    }
}
