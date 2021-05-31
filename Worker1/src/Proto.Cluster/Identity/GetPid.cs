using System.Threading;
using Proto.Router;

namespace Proto.Cluster.Identity
{
    class GetPid : IHashable
    {
        public GetPid(ClusterIdentity clusterIdentity, CancellationToken cancellationToken, string tick)
        {
            Tick = tick;
            ClusterIdentity = clusterIdentity;
            CancellationToken = cancellationToken;
        }

        public string Tick { get; }
        public ClusterIdentity ClusterIdentity { get; }
        public CancellationToken CancellationToken { get; }

        public string HashBy() => ClusterIdentity.ToString();
    }

    class PidResult
    {
        public PID? Pid { get; }

        public PidResult(PID? pid) => Pid = pid;
    }
}