using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM2.Shared.Shared.ProtoActorRuntime
{
    public interface IProtoActorBuilder
    {
        IProtoActorBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate);
    }
    internal class ProtoActorBuilder : IProtoActorBuilder
    {
        private readonly IHostBuilder hostBuilder;
        private readonly List<Action<HostBuilderContext, IProtoActorBuilder>> configureClusterDelegates = new List<Action<HostBuilderContext, IProtoActorBuilder>>();
        private readonly List<Action<HostBuilderContext, IServiceCollection>> configureServicesDelegates = new List<Action<HostBuilderContext, IServiceCollection>>();

        public ProtoActorBuilder(IHostBuilder hostBuilder)
        {
            this.hostBuilder = hostBuilder;
        }

        public void Build(Microsoft.Extensions.Hosting.HostBuilderContext context, IServiceCollection serviceCollection)
        {
            foreach (var configurationDelegate in this.configureClusterDelegates)
            {
                configurationDelegate(context, this);
            }

            serviceCollection.AddHostedService<ProtoActorHostedService>();
            this.ConfigureDefaults();

            foreach (var configurationDelegate in this.configureServicesDelegates)
            {
                configurationDelegate(context, serviceCollection);
            }
        }

        public IProtoActorBuilder ConfigureCluster(Action<Microsoft.Extensions.Hosting.HostBuilderContext, IProtoActorBuilder> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
            this.configureClusterDelegates.Add(configureDelegate);
            return this;
        }

        public IProtoActorBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));
            this.configureServicesDelegates.Add(configureDelegate);
            return this;
        }
    }

    
}
