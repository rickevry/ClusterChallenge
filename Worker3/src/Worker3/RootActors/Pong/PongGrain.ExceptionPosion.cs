using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using PoisonPill;

namespace Worker2
{
    public partial class PongGrain 
    {

        public Task HandleExceptionPoison(IContext context, ExceptionPoison cmd)
        {
            logger.LogInformation($"PongGrain.HandleExceptionPoison");

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

