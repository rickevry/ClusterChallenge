using DAM2.Core.Shared.Interface;
using Microsoft.Extensions.Logging;
using PingPong;
using Proto.Cluster;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Worker2
{
    public class MainWorker : IMainWorker
    {

        private readonly ILogger<MainWorker> logger;

        public MainWorker(ILogger<MainWorker> logger)
        {
            this.logger = logger;
        }

        public async Task Run(Cluster _cluster)
        {

            this.logger.LogInformation("MainWorker start");

            int errors = 0;
            int counter = 1;
            string kind = "pong";
            await Task.Delay(5000);

            while (true)
            {
                try
                {
                    var cmd = new PongCmd { Counter = counter };
                    var res = await _cluster.RequestAsync<PongResponse>("Worker2/101", kind, cmd, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                if (true.Equals(res?.Success))
                    {
                        counter = res.Counter;
                        this.logger.LogInformation("Pong counter is {counter}", counter);
                    }
                    else
                    {
                        errors++;
                        this.logger.LogInformation("Failed to call {Kind}'", kind);
                    }
                    await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e.ToString());
                }
            }
        }
    }
}
