using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shashlik.RC.Config.Demo.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class DemoController : ControllerBase
    {
        [HttpGet("/configuration")]
        public object Configuration([FromServices] IConfiguration configuration)
        {
            return configuration.GetSection("Test").Get<IDictionary<string, string>>();
        }

        [HttpGet("/Snapshot")]
        public object Snapshot([FromServices] IOptionsSnapshot<TestOptions> optionsSnapshot)
        {
            return optionsSnapshot.Value;
        }

        [HttpGet("Monitor")]
        public object Monitor([FromServices] IOptionsMonitor<TestOptions> optionsMonitor)
        {
            return optionsMonitor.CurrentValue;
        }
    }
}