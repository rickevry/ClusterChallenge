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

        public Task HandleExceptionPoison(IContext context, ExceptionPoison cmd)
        {
            logger.LogInformation($"PingGrain.HandleExceptionPoison");

            try
            {
                this.exceptionCounter++;
                context.Respond(new PoisonResponse());
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in HandleExceptionPoison");
                context.Respond(new PoisonResponse());
            }
            return Task.CompletedTask;
        }

    }
}

