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
	[Actor("ping")]
    public partial class PingGrain : GenericActor, IActor
    {
        private readonly ILogger<PingGrain> logger;
        private int exceptionCounter = 0;
        private int sleepCounter = 0;
        private int noAnswerCounter = 0;
        public PingGrain(ILogger<PingGrain> logger)
        {
	        this.logger = logger;
        }

        public override async Task ReceiveAsync(IContext context)
        {
	        Task task = context.Message switch
	        {
                PingCmd cmd => HandlePing(context, cmd),
                ExceptionPoison cmd => HandleExceptionPoison(context, cmd),
                NoAnswerPoison cmd => HandleNoAnswerPoison(context, cmd),
                SleepPoison cmd => HandleSleepPoison(context, cmd),

                Started _ => Started(context),
		        _ => base.ReceiveAsync(context)
	        };
            
            try
	        {
		        await task;
            }
	        catch (Exception e)
	        {
		        this.logger.LogError(e, "Failed PingGrain");
                context.Respond(new DeadLetterResponse
                {
                    Target = context.Self
                });
	        }
        }


        public async Task HandlePing(IContext context, PingCmd cmd)
        {
	        logger.LogInformation($"PingGrain.HandlePing");

            if (this.exceptionCounter > 0)
            {
                this.exceptionCounter--;
                throw new Exception("Exception poison");
            }


            try
            {
                if (this.noAnswerCounter > 0)
                {
                    this.noAnswerCounter--;
                    return;
                }

                if (this.sleepCounter > 0)
                {
                    this.sleepCounter--;
                    await Task.Delay(10 * 1000);
                }

		        context.Respond(new PingResponse
                {
			        Success = true,
                    Counter = cmd.Counter + 1
                });
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in HandlePing");
                context.Respond(new PingResponse() { Success = false, ErrorMessage = "Internal server error"});
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
                    logger.LogInformation("PingGrain.Started with: {@PidValues}", pidValues);
                }
            }
            catch (Exception e)
            {
	            logger.LogError(e, "PingGrain failed");
            }
        }
    }
}

