using Proto.Cluster;

namespace DAM2.Core.Shared.Interface
{
    public interface ISharedSetupRootActors
    {
        ClusterConfig AddRootActors(ClusterConfig clusterConfig);

    }
}
