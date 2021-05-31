using System.Threading.Tasks;
using Proto;

namespace DAM2.Core.Shared.Subscriptions
{
    public interface ISubscription
    {
	    Task Subscribe(EventStream eventStream);
    }
}
