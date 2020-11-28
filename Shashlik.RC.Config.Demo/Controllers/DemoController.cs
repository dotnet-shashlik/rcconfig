using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Config.Demo.Controllers
{
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet("/demo/{key}")]
        public object Test([FromServices] IConfiguration configuration, string key)
        {
            return configuration.GetValue<string>(key);
        }
    }
}