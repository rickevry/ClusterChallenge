using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DAM2.Core.Shared.Subscriptions
{
	public interface ISubscriptionMediator
	{
		Task Handle<TCommand>(TCommand command);
	}

    public class SubscriptionMediator : ISubscriptionMediator
    {
	    private readonly IServiceProvider serviceProvider;
	    private readonly ILogger<ISubscriptionMediator> logger;

	    public SubscriptionMediator(IServiceProvider serviceProvider, ILogger<ISubscriptionMediator> logger)
	    {
		    this.serviceProvider = serviceProvider;
		    this.logger = logger;
	    }
	    public async Task Handle<TCommand>(TCommand command)
	    {
			this.logger.LogInformation("Message Received: {@Message}", command);
		    using var scope = this.serviceProvider.CreateScope();

		    ISubscriptionHandler<TCommand> handler = scope.ServiceProvider.GetRequiredService<ISubscriptionHandler<TCommand>>();
		    try
		    {
			    await handler.Handle(command).ConfigureAwait(false);
			}
		    catch (Exception e)
		    {
			    this.logger.LogError(e, "Message handler failed");
		    }
	    }
    }
}
