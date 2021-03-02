using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using DAM2.Core.Actors.Shared.Generic;
using DAM2.Core.Shared.Generic;
using PingPong;
using PoisonPill;

namespace Worker1
{
    public partial class PingGrain 
    {

        public Task HandleSleepPoison(IContext context, SleepPoison cmd)
        {
            logger.LogInformation($"PingGrain.HandleSleepPoison");

            try
            {
                this.sleepCounter++;
                context.Respond(new PoisonResponse());
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in HandleSleepPoison");
                context.Respond(new PingResponse());
            }
            return Task.CompletedTask;
        }

    }
}

