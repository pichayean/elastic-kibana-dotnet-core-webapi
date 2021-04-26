using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Webapi_Serilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KibanaLogController : ControllerBase
    {
        private readonly ILogger<KibanaLogController> _logger;

        public KibanaLogController(ILogger<KibanaLogController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("CreateInformationLog")]
        public IActionResult Get()
        {
            _logger.LogInformation("_Log Testing {date}", DateTime.UtcNow);
            return Ok(new { status = "Hello" });
        }
    }
}
