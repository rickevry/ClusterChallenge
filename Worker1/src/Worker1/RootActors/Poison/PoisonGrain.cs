using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using DAM2.Core.Actors.Shared.Generic;
using DAM2.Core.Shared.Generic;
using PoisonPill;

namespace Worker1
{
	[Actor("poison1")]
    public partial class PoisonGrain : GenericActor, IActor
    {
        private readonly ILogger<PoisonGrain> logger;

        public PoisonGrain(ILogger<PoisonGrain> logger)
        {
	        this.logger = logger;
        }

        public override async Task ReceiveAsync(IContext context)
        {
	        Task task = context.Message switch
	        {
                BroadcastPoison cmd => HandleBroadcastPoison(context, cmd),
                RestartPoison cmd => HandleRestartPoison(context, cmd),
                Started _ => Started(context),
		        _ => base.ReceiveAsync(context)
	        };
            
            try
	        {
		        await task;
            }
	        catch (Exception e)
	        {
		        this.logger.LogError(e, "Failed PoisonGrain");
                context.Respond(new DeadLetterResponse
                {
                    Target = context.Self
                });
	        }
        }

      

        protected override async Task Started(IContext context)
        {
	        logger.LogInformation($"{context.Self.Address} - Started");
            try
            {
                await base.Started(context);
                if (pidValues != PIDValues.Empty)
                {
                    logger.LogInformation("PoisonGrain.Started with: {@PidValues}", pidValues);
                }
            }
            catch (Exception e)
            {
	            logger.LogError(e, "PoisonGrain failed");
            }
        }
    }
}

