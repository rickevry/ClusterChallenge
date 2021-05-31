using Microsoft.Extensions.Logging;
using Proto.Cluster;

namespace DAM2.Core.Shared.Interface
{
    public interface ISharedClusterProviderFactory
    {
        IClusterProvider CreateClusterProvider(ILogger _logger);
    }
}
