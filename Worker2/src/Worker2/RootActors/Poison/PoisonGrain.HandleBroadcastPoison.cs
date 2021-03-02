using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using DAM2.Core.Actors.Shared.Generic;
using DAM2.Core.Shared.Generic;
using PingPong;
using PoisonPill;

namespace Worker2
{
    public partial class PoisonGrain
    {

        public Task HandleBroadcastPoison(IContext context, BroadcastPoison cmd)
        {
	        logger.LogInformation($"PoisonGrain.HandleBroadcastPoison");

            try
            {
		        context.Respond(new PoisonResponse());
		        return Task.CompletedTask;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in PoisonGrain");
                context.Respond(new PoisonResponse());
            }
            return Task.CompletedTask;
        }

    }
}

