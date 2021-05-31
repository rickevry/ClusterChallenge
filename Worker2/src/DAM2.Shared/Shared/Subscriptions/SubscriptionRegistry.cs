using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DAM2.Core.Shared.Subscriptions
{
    public static class SubscriptionRegistry
    {
	    public static IServiceCollection AddSubscriptions(this IServiceCollection services, params Assembly[]? subscriptionAssemblies)
	    {
			services.TryAddSingleton<ISubscriptionMediator, SubscriptionMediator>();
			services.TryAddTransient<ISubscriptionFactory, SubscriptionFactory>();

			if (subscriptionAssemblies == null || !subscriptionAssemblies.Any())
			{
				subscriptionAssemblies = new Assembly[] {typeof(SubscriptionRegistry).Assembly};
			}

			services.Scan(scan =>
			{
				scan.FromAssemblies(subscriptionAssemblies)
					.AddClasses(classes => classes.AssignableTo<ISubscription>())
						.AsImplementedInterfaces()
						.WithSingletonLifetime()
					.AddClasses(classes => classes.AssignableTo(typeof(ISubscriptionHandler<>)))
						.AsImplementedInterfaces()
						.WithTransientLifetime();
			});

			return services;
	    }
    }
}
