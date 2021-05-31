using Proto.Cluster;
using System.Threading.Tasks;

namespace DAM2.Core.Shared.Interface
{
    public interface IMainWorker
    {
        Task Run(Cluster cluster);
    }
}
