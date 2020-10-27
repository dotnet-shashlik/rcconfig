using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Shashlik.RC.Config.Demo.Controllers
{
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet("/demo")]
        public object Test([FromServices] IConfiguration configuration)
        {
            return configuration.GetValue<string>("Demo");
        }
    }
}
