using System.Threading.Tasks;
using Proto.Cluster;

namespace DAM2.Core.Shared.Interface
{
    public interface ISharedClusterClient
    {
	    bool Connected { get; }
        Cluster Cluster { get; }
        Task<T> RequestAsync<T>(string actorPath, string clusterKind, object cmd);
        Task Startup();
        Task Shutdown();
    }
}
