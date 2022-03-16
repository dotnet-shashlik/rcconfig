using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Log;
using Shashlik.RC.Server.Services.Log.Dtos;
using Shashlik.RC.Server.Services.Log.Inputs;

namespace Shashlik.RC.Server.Controllers
{
    public class LogsController : ApiControllerBase
    {
        public LogsController(LogService logService)
        {
            LogService = logService;
        }

        private LogService LogService { get; }

        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task<PageModel<LogListDto>> Get([FromQuery] SearchLogInput input)
        {
            return await LogService.List(GetResourceId(), input);
        }

        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/{logId:int:min(1)}")]
        public async Task<LogDetailDto> Get(int logId)
        {
            var log = await LogService.Get(GetResourceId(), logId);
            return log ?? throw ResponseException.NotFound();
        }
    }
}