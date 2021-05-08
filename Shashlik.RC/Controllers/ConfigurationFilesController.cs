using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
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
        public async Task<PageModel<ConfigurationFileDto>> Get([FromQuery] PageInput input)
        {
            return await ConfigurationFileService.List(GetResourceId(), input);
        }

        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Post(CreateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Create(GetResourceId(), input);
        }

        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Patch(int fileId, UpdateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Update(GetResourceId(), fileId, input);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Delete(int fileId)
        {
            //TODO: event
            await ConfigurationFileService.Delete(GetResourceId(), fileId);
        }
    }
}