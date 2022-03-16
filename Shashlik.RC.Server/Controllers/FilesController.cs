using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.EventBus;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.ConfigurationFile;
using Shashlik.RC.Server.Services.ConfigurationFile.Dtos;
using Shashlik.RC.Server.Services.ConfigurationFile.Inputs;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Controllers
{
    public class FilesController : ApiControllerBase
    {
        public FilesController(FileService configurationFileService)
        {
            ConfigurationFileService = configurationFileService;
        }

        private FileService ConfigurationFileService { get; }

        /// <summary>
        /// 分页获取文件数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task<PageModel<ConfigurationFileListDto>> Get([FromQuery] PageInput input)
        {
            return await ConfigurationFileService.List(GetResourceId(), input);
        }

        /// <summary>
        /// 获取单个文件数据
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task<ConfigurationFileDto> Get(int fileId)
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
            return await EventQueue.Wait(GetResourceId(), version, cancellation);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Post(CreateConfigurationFileInput input)
        {
            await ConfigurationFileService.Create(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), input);
            EventQueue.OnUpdate(GetResourceId(), DateTime.Now.GetLongDate());
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Patch(int fileId, UpdateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Update(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId, input);
            EventQueue.OnUpdate(GetResourceId(), DateTime.Now.GetLongDate());
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Delete(int fileId)
        {
            //TODO: event
            await ConfigurationFileService.Delete(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId);
            EventQueue.OnUpdate(GetResourceId(), DateTime.Now.GetLongDate());
        }
    }
}