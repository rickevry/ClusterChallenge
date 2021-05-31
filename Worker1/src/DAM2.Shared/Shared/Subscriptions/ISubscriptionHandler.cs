using System.Threading.Tasks;

namespace DAM2.Core.Shared.Subscriptions
{
    public interface ISubscriptionHandler<in TCommand>
    {
	    Task Handle(TCommand updateEventCmd);
    }
}
