using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using DAM2.Core.Shared.Interface;
using DAM2.Core.Actors.Shared.Generic;

namespace Worker1
{
    public class SetupRootActors : ISharedSetupRootActors
    {
	    private readonly IServiceProvider _serviceProvider;
	    private readonly ILogger<SetupRootActors> logger;

        public SetupRootActors(IServiceProvider serviceProvider, ILogger<SetupRootActors> logger)
        {
	        _serviceProvider = serviceProvider;
	        this.logger = logger;
        }
        public ClusterConfig AddRootActors(ClusterConfig clusterConfig)
        {
	        List<Type> actorTypes = typeof(SetupRootActors).Assembly.GetTypes().Where(t =>
	        {
		        if (t.IsInterface)
		        {
			        return false;
		        }
		        var typeInfo = t.GetTypeInfo();
		        var otherTypeInfo = typeof(IActor).GetTypeInfo();
		        return otherTypeInfo.IsAssignableFrom(typeInfo);
	        }).ToList();
	        foreach (Type actorType in actorTypes)
	        {
		        var typeInfo = actorType.GetTypeInfo();
		        ActorAttribute? actorAttribute = typeInfo.GetCustomAttribute<ActorAttribute>();
                if(actorAttribute != null && !string.IsNullOrWhiteSpace(actorAttribute.Kind))
                {
	                var kind = actorAttribute.Kind;
	                var props = Props.FromProducer(() => (IActor)this._serviceProvider.GetRequiredService(actorType));
	                clusterConfig = clusterConfig.WithClusterKind(kind, props);

	                this.logger.LogInformation("'{Type}' is set up FromProducer with Kind '{Kind}'", actorType.Name, kind);
                }
                else
                {
                    this.logger.LogWarning("'{Type}' must have ActorAttribute with a Kind value to be activated. Will be ignored.", actorType.Name);
                }
	        }
            return clusterConfig;
        }
    }
}
