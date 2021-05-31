using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Proto.Cluster.Identity
{
    public class IdentityStorageLookup : IIdentityLookup
    {
        private const string PlacementActorName = "placement-activator";
        private static readonly int PidClusterIdentityStartIndex = PlacementActorName.Length + 1;
        private bool _isClient;
        private string _memberId;
        private PID _placementActor;
        private ActorSystem _system;
        private PID _worker;
        internal Cluster Cluster;
        internal MemberList MemberList;
        private ILogger _logger = null!;

        public IdentityStorageLookup(IIdentityStorage storage) => Storage = storage;

        internal IIdentityStorage Storage { get; }

        public async Task<PID?> GetAsync(ClusterIdentity clusterIdentity, CancellationToken ct, string tick="")
        {

            _logger.LogDebug("[Rickard] GetAsync XXX" + clusterIdentity.Identity + " " + tick);
            var msg = new GetPid(clusterIdentity, ct, tick);

            var res = await _system.Root.RequestAsync<PidResult>(_worker, msg, ct);

            // Rickard: kommer aldrig tillbaka hit!!!

            _logger.LogDebug("[Rickard] GetAsync result XXX: + " + tick + " " + res?.Pid);
            return res?.Pid;
        }

        public Task SetupAsync(Cluster cluster, string[] kinds, bool isClient)
        {
            Cluster = cluster;
            _system = cluster.System;
            _memberId = cluster.System.Id;
            MemberList = cluster.MemberList;
            _isClient = isClient;
            _logger = Log.CreateLogger(nameof(IdentityStorageLookup));

            var workerProps = Props.FromProducer(() => new IdentityStorageWorker(this));
            _worker = _system.Root.Spawn(workerProps);

            //hook up events
            cluster.System.EventStream.Subscribe<ClusterTopology>(e => {
                    //delete all members that have left from the lookup
                    foreach (var left in e.Left)
                        //YOLO. event stream is not async
                    {
                        _ = RemoveMemberAsync(left.Id);
                    }
                }
            );

            if (isClient) return Task.CompletedTask;

            var props = Props.FromProducer(() => new IdentityStoragePlacementActor(Cluster, this));
            _placementActor = _system.Root.SpawnNamed(props, PlacementActorName);

            return Task.CompletedTask;
        }

        public async Task ShutdownAsync()
        {
            await Cluster.System.Root.StopAsync(_worker);
            if (!_isClient) await Cluster.System.Root.StopAsync(_placementActor);

            await RemoveMemberAsync(_memberId);
        }

        public Task RemovePidAsync(PID pid, CancellationToken ct)
        {
            if (_system.Shutdown.IsCancellationRequested) return Task.CompletedTask;

            return Storage.RemoveActivation(pid, ct);
        }

        internal Task RemoveMemberAsync(string memberId) => Storage.RemoveMember(memberId, CancellationToken.None);

        internal PID RemotePlacementActor(string address) => PID.FromAddress(address, PlacementActorName);

        public static bool TryGetClusterIdentityShortString(string pidId, out string? clusterIdentity)
        {
            var idIndex = pidId.LastIndexOf("$", StringComparison.Ordinal);

            if (idIndex > PidClusterIdentityStartIndex)
            {
                clusterIdentity = pidId.Substring(PidClusterIdentityStartIndex,
                    idIndex - PidClusterIdentityStartIndex
                );
                return true;
            }

            clusterIdentity = default;
            return false;
        }
    }
}