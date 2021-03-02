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
	[Actor("pong")]
    public partial class PongGrain : GenericActor, IActor
    {
        private readonly ILogger<PongGrain> logger;
        private int exceptionCounter = 0;
        private int sleepCounter = 0;
        private int noAnswerCounter = 0;
        public PongGrain(ILogger<PongGrain> logger)
        {
	        this.logger = logger;
        }

        public override async Task ReceiveAsync(IContext context)
        {
	        Task task = context.Message switch
	        {
                PongCmd cmd => HandlePong(context, cmd),
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
		        this.logger.LogError(e, "Failed PongGrain");
                context.Respond(new DeadLetterResponse
                {
                    Target = context.Self
                });
	        }
        }


        public async Task HandlePong(IContext context, PongCmd cmd)
        {
	        logger.LogInformation($"PongGrain.HandlePong");

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

		        context.Respond(new PongResponse
                {
			        Success = true,
                    Counter = cmd.Counter + 1
                });
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed in PongGrain");
                context.Respond(new PongResponse() { Success = false, ErrorMessage = "Internal server error"});
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
                    logger.LogInformation("PongGrain.Started with: {@PidValues}", pidValues);
                }
            }
            catch (Exception e)
            {
	            logger.LogError(e, "PongGrain failed");
            }
        }
    }
}

