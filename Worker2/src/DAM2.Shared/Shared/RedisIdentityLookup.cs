using Proto.Cluster.Identity;
using Proto.Cluster.Identity.Redis;
using StackExchange.Redis;

namespace DAM2.Core.Shared
{
    public class RedisIdentityLookup
    {
        public static IIdentityLookup GetIdentityLookup(string clusterName, string host, string port)
        {
            ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect($"{host}:{port}");
            var identity = new IdentityStorageLookup(new RedisIdentityStorage(clusterName, muxer));
            return identity;
        }
    }
}
