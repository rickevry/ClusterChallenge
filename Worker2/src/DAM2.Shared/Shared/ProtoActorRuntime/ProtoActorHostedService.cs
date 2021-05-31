using DAM2.Core.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto.Cluster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAM2.Shared.Shared.ProtoActorRuntime
{
    public class ProtoActorHostedService : IHostedService
    {
        private readonly SharedClusterWorker cluster;
        private readonly ILogger<ProtoActorHostedService> logger;

        public ProtoActorHostedService(SharedClusterWorker cluster, ILogger<ProtoActorHostedService> logger)
        {
            this.cluster = cluster;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Starting ProtoActor Cluster.");
            await this.cluster.Run();
            this.logger.LogInformation("ProtoActor Cluster stared.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Stopping ProtoActor Cluster.");
            await this.cluster.Shutdown().ConfigureAwait(false);
            this.logger.LogInformation("ProtoActor Cluster stopped.");
        }
    }
}
