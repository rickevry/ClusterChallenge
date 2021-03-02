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
        public Task HandleRestartPoison(IContext context, RestartPoison cmd)
        {
            logger.LogInformation($"PoisonGrain.RestartPoison");

            try
            {
                context.Respond(new PoisonResponse());
                //Environment.Exit(-1);
                ForceShutdown.appLifetime.StopApplication();
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

