using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DAM2.Core.Shared.Interface;
using Status;
using PoisonPill;

namespace Client1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClusterController : ControllerBase
    {
        private readonly ILogger<ClusterController> _logger;
        private readonly ISharedClusterClient _clusterClient;

        private const string WORKER1KIND = "worker1";
        private const string WORKER2KIND = "worker2";

        public ClusterController(ILogger<ClusterController> logger, ISharedClusterClient clusterClient)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }

        [HttpGet]
        [Route("getStatus")]
        public async Task<ActionResult> GetStatus()
        {
            var cmd = new GetStatusCmd();
            var res1 = await _clusterClient.RequestAsync<GetStatusResponse>("/", WORKER1KIND, cmd);
            var res2 = await _clusterClient.RequestAsync<GetStatusResponse>("/", WORKER2KIND, cmd);
            return Ok(new { id = 123 });
        }

        private string GetKind(int worker)
        {
            return (worker == 1) ? WORKER1KIND : WORKER2KIND;
        }

        [HttpGet]
        [Route("sendBroadcastPoison")]
        public async Task<ActionResult> SendBroadcastPoison(int worker)
        {
            var cmd = new BroadcastPoison();
            var res1 = await _clusterClient.RequestAsync<PoisonResponse>("/", GetKind(worker), cmd);
            return Ok();
        }

        [HttpGet]
        [Route("sendRestartPoison")]
        public async Task<ActionResult> SendRestartPoison(int worker)
        {
            var cmd = new BroadcastPoison();
            var res1 = await _clusterClient.RequestAsync<PoisonResponse>("/", GetKind(worker), cmd);
            return Ok();
        }

        [HttpGet]
        [Route("sendExceptionPoison")]
        public async Task<ActionResult> SendExceptionPoison(int worker)
        {
            var cmd = new BroadcastPoison();
            var res1 = await _clusterClient.RequestAsync<PoisonResponse>("/", GetKind(worker), cmd);
            return Ok();
        }

        [HttpGet]
        [Route("sendNoAnswerPoison")]
        public async Task<ActionResult> SendNoAnswerPoison(int worker)
        {
            var cmd = new BroadcastPoison();
            var res1 = await _clusterClient.RequestAsync<PoisonResponse>("/", GetKind(worker), cmd);
            return Ok();
        }

        [HttpGet]
        [Route("sendSleepPoison")]
        public async Task<ActionResult> SendSleepPoison(int worker)
        {
            var cmd = new BroadcastPoison();
            var res1 = await _clusterClient.RequestAsync<PoisonResponse>("/", GetKind(worker), cmd);
            return Ok();
        }

    }
}

