using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Log;
using Shashlik.RC.Services.Log.Dtos;
using Shashlik.RC.Services.Log.Inputs;
using Shashlik.Response;

namespace Shashlik.RC.Controllers
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