using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.RC.Server.Services.Environment.Dtos;
using Shashlik.RC.Server.Services.Environment.Inputs;
using Shashlik.RC.Server.Services.Resource;
using Shashlik.RC.Server.Services.Permission;

namespace Shashlik.RC.Server.Controllers
{
    public class EnvironmentsController : ApiControllerBase
    {
        public EnvironmentsController(EnvironmentService environmentService, ResourceService permissionService)
        {
            EnvironmentService = environmentService;
            PermissionService = permissionService;
        }

        private EnvironmentService EnvironmentService { get; }
        private ResourceService PermissionService { get; }

        [HttpGet(Constants.ResourceRoute.Application)]
        public async Task<List<EnvironmentDto>> Get()
        {
            var list = await EnvironmentService.List(GetResourceId());
            return (await PermissionService.DoFilterIsAdminFromContext(LoginUserId!.Value, list)).ToList();
        }

        [HttpPost(Constants.ResourceRoute.Application)]
        public async Task Post(CreateEnvironmentInput input)
        {
            await EnvironmentService.Create(GetResourceId(), input);
        }

        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Patch(UpdateEnvironmentInput input)
        {
            await EnvironmentService.Update(GetResourceId(), input);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Delete()
        {
            await EnvironmentService.Delete(GetResourceId());
        }
    }
}