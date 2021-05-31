using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using PoisonPill;

namespace Worker2
{
    public partial class PongGrain 
    {

        public Task HandleSleepPoison(IContext context, SleepPoison cmd)
        {
            logger.LogInformation($"PongGrain.HandleSleepPoison");

            try
            {
                this.sleepCounter++;
                context.Respond(new PoisonResponse());
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in HandleSleepPoison");
                context.Respond(new PoisonResponse());
            }
            return Task.CompletedTask;
        }

    }
}

