using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.ConfigurationFile;
using Shashlik.RC.Services.ConfigurationFile.Dtos;
using Shashlik.RC.Services.ConfigurationFile.Inputs;

namespace Shashlik.RC.Controllers
{
    public class ConfigurationFilesController : ApiControllerBase
    {
        public ConfigurationFilesController(ConfigurationFileService configurationFileService)
        {
            ConfigurationFileService = configurationFileService;
        }

        private ConfigurationFileService ConfigurationFileService { get; }

        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task<PageModel<ConfigurationFileListDto>> Get([FromQuery] PageInput input)
        {
            return await ConfigurationFileService.List(GetResourceId(), input);
        }

        [HttpGet(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task<ConfigurationFileDto> Get(int fileId)
        {
            var file = await ConfigurationFileService.Get(GetResourceId(), fileId);
            return file ?? throw ResponseException.NotFound();
        }

        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Post(CreateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Create(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), input);
        }

        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Patch(int fileId, UpdateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Update(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId, input);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Delete(int fileId)
        {
            //TODO: event
            await ConfigurationFileService.Delete(LoginUserId!.Value, User.Identity!.Name!, GetResourceId(), fileId);
        }
    }
}