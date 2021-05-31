using Proto;

namespace DAM2.Core.Shared.Interface
{
    public interface IClusterActor : IActor
    {
        public string ClusterKind { get; }
    }
}
