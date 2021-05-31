using DAM2.Core.Shared.Interface;
using Microsoft.Extensions.Logging;
using PingPong;
using Proto.Cluster;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker1
{
    public class MainWorker : IMainWorker
    {

        private readonly ILogger<MainWorker> logger;
        int errors = 0;
        int pingCounter = 1;
        int pongCounter = 1;

        public MainWorker(ILogger<MainWorker> logger)
        {
            this.logger = logger;
        }


        private async Task Ping(Cluster _cluster)
        {
            try
            {
                string tick = DateTime.Now.Ticks.ToString();
                string kind = "ping";
                var cmd = new PingCmd { Counter = pingCounter };
                var res = await _cluster.RequestAsync<PingResponse>("Worker1/101", kind, cmd, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token, tick);
                if (true.Equals(res?.Success))
                {
                    pingCounter = res.Counter;
                    this.logger.LogInformation("Ping counter is {counter}", pingCounter);
                }
                else
                {
                    errors++;
                    this.logger.LogInformation("Failed to call {Tick} {Kind} {errors} '", tick, kind, errors);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
            }
        }

        private async Task Pong(Cluster _cluster)
        {
            try
            {
                string kind = "pong";
                string tick = DateTime.Now.Ticks.ToString();

                var cmd = new PongCmd { Counter = pongCounter };
                this.logger.LogInformation("Calling Pong", tick);
                var res = await _cluster.RequestAsync<PongResponse>("Worker2/101", kind, cmd, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token, tick);
                if (true.Equals(res?.Success))
                {
                    pongCounter = res.Counter;
                    this.logger.LogInformation("Pong counter is {counter}", pongCounter);
                }
                else
                {
                    errors++;
                    this.logger.LogInformation("Failed to call {Tick} {Kind} {errors} '", tick, kind, errors);

                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
            }
        }

        public async Task Run(Cluster _cluster)
        {

            this.logger.LogInformation("MainWorker start 2021-03-04 14:55");
            await Task.Delay(5000);

            while (true)
            {
                await Ping(_cluster);
                await Pong(_cluster);
                await Task.Delay(1000);
            }
        }
    }
}
