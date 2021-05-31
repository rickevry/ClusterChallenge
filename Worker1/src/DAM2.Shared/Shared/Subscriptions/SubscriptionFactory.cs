using System.Collections.Generic;
using System.Threading.Tasks;
using Proto;

namespace DAM2.Core.Shared.Subscriptions
{
	public interface ISubscriptionFactory
	{
		Task FireUp(ActorSystem actorSystem);
	}

    public class SubscriptionFactory : ISubscriptionFactory
    {
	    private readonly IEnumerable<ISubscription> subscriptions;

	    public SubscriptionFactory(IEnumerable<ISubscription> subscriptions)
	    {
		    this.subscriptions = subscriptions;
	    }
	    public Task FireUp(ActorSystem actorSystem)
	    {
		    foreach (var subscription in this.subscriptions)
		    {
			    subscription.Subscribe(actorSystem.EventStream);
		    }

			return Task.CompletedTask;
	    }
    }
}
