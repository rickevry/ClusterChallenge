using DAM2.Shared.Shared.ProtoActorRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseProtoActorCluster(this IHostBuilder hostBuilder, Action<HostBuilderContext, IProtoActorBuilder> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
            const string protoActorBuilderKey = "ProtoActorBuilder";
            ProtoActorBuilder builder = new ProtoActorBuilder(hostBuilder);
            if (!hostBuilder.Properties.ContainsKey(protoActorBuilderKey))
            {
                hostBuilder.Properties.Add(protoActorBuilderKey, builder);
                builder.ConfigureServices((context, services) =>
                {
                    builder.Build(context, services);
                });
            }
            else
            {
                builder = (ProtoActorBuilder)hostBuilder.Properties[protoActorBuilderKey];
            }

            builder.ConfigureCluster(configureDelegate);
            return hostBuilder;
        }

        public static IHostBuilder UseProtoActorCluster(this IHostBuilder hostBuilder, Action<IProtoActorBuilder> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
            return hostBuilder.UseProtoActorCluster((ctx, siloBuilder) => configureDelegate(siloBuilder));
        }
    }
}
