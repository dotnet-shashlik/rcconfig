using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shashlik.RC.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    public class ApiControllerBase : ControllerBase
    {
    }
}