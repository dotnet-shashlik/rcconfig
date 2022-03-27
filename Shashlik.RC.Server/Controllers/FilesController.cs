using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.EventBus;
using Shashlik.RC.Server.Monitor;
using Shashlik.RC.Server.Services.File;
using Shashlik.RC.Server.Services.File.Inputs;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.RC.Server.Services.File.Dtos;

namespace Shashlik.RC.Server.Controllers
{
    public class FilesController : ApiControllerBase
    {
        public FilesController(FileService configurationFileService, ResourceMonitor resourceMonitor, EnvironmentService environmentService)
        {
            ConfigurationFileService = configurationFileService;
            ResourceMonitor = resourceMonitor;
            EnvironmentService = environmentService;
        }

        private FileService ConfigurationFileService { get; }
        private ResourceMonitor ResourceMonitor { get; }
        private EnvironmentService EnvironmentService { get; }

        /// <summary>
        /// 分页获取文件数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task<PageModel<FileListDto>> Get([FromQuery] PageInput input)
        {
            return await ConfigurationFileService.List(GetResourceId(), input);
        }

        /// <summary>
        /// 获取单个文件数据
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task<FileDto> Get(int fileId)
        {
            var file = await ConfigurationFileService.Get(GetResourceId(), fileId);
            return file ?? throw ResponseException.NotFound();
        }

        /// <summary>
        /// 获取所有的配置数据
        /// </summary>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/all")]
        public async Task<object> All([FromServices] EnvironmentService environmentService)
        {
            var environmentDto = await environmentService.Get(GetResourceId());
            return new { Files = await ConfigurationFileService.All(GetResourceId()), environmentDto!.Version };
        }

        /// <summary>
        /// 长轮询检测是否有文件变动
        /// </summary>
        /// <param name="version"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/poll")]
        public async Task<bool> Pool(long version, CancellationToken cancellation)
        {
            return await ResourceMonitor.WaitUpdate(GetResourceId(), version, cancellation);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Post(CreateFileInput input)
        {
            await ConfigurationFileService.Create(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), input);
            HttpContext.RequestServices.Dispatch(Constants.Events.ResourceUpdated, await EnvironmentService.Get(GetResourceId()));
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Patch(int fileId, UpdateFileInput input)
        {
            await ConfigurationFileService.Update(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId, input);
            HttpContext.RequestServices.Dispatch(Constants.Events.ResourceUpdated, await EnvironmentService.Get(GetResourceId()));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Delete(int fileId)
        {
            await ConfigurationFileService.Delete(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId);
            HttpContext.RequestServices.Dispatch(Constants.Events.ResourceUpdated, await EnvironmentService.Get(GetResourceId()));
        }
    }
}