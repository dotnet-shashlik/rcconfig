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
        public async Task<PageModel<ConfigurationFileDto>> Get(string env, [FromQuery] PageInput input)
        {
            return await ConfigurationFileService.List(env, input);
        }

        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Post(string env, CreateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Create(env, input);
        }

        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Patch(string env, int fileId, UpdateConfigurationFileInput input)
        {
            //TODO: event
            await ConfigurationFileService.Update(env, fileId, input);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/{fileId:int:min(1)}")]
        public async Task Delete(string env, int fileId)
        {
            //TODO: event
            await ConfigurationFileService.Delete(env, fileId);
        }
    }
}