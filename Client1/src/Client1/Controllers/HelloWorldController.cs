using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client1
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {

        public HelloWorldController()
        {
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Hello world!");
        }

    }
}
