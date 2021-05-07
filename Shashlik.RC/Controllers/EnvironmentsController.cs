using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Environment;
using Shashlik.RC.Services.Environment.Dtos;
using Shashlik.RC.Services.Environment.Inputs;
using Shashlik.RC.Services.Permission;

namespace Shashlik.RC.Controllers
{
    public class EnvironmentsController : ApiControllerBase
    {
        public EnvironmentsController(EnvironmentService environmentService, PermissionService permissionService)
        {
            EnvironmentService = environmentService;
            PermissionService = permissionService;
        }

        private EnvironmentService EnvironmentService { get; }
        private PermissionService PermissionService { get; }

        [HttpGet]
        public async Task<List<EnvironmentDto>> Get()
        {
            var list = await EnvironmentService.List();
            return (await PermissionService.DoFilter(LoginUserId!.Value, list)).ToList();
        }

        [HttpPost(Constants.ResourceRoute.Application)]
        public async Task Post(string app, CreateEnvironmentInput input)
        {
            await EnvironmentService.Create(app, input);
        }

        [HttpPatch(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Patch(string env, UpdateEnvironmentInput input)
        {
            await EnvironmentService.Update(env, input);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment)]
        public async Task Delete(string env)
        {
            await EnvironmentService.Delete(env);
        }
    }
}