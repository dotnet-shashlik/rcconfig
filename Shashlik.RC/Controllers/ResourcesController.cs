using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Application;
using Shashlik.RC.Services.Environment;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Permission.Inputs;

namespace Shashlik.RC.Controllers
{
    public class ResourcesController : ApiControllerBase
    {
        public ResourcesController(ApplicationService applicationService, EnvironmentService environmentService, PermissionService permissionService)
        {
            ApplicationService = applicationService;
            EnvironmentService = environmentService;
            PermissionService = permissionService;
        }

        private ApplicationService ApplicationService { get; }
        private EnvironmentService EnvironmentService { get; }
        private PermissionService PermissionService { get; }

        [HttpGet]
        public async Task<IEnumerable<string>> List()
        {
            var applications = await ApplicationService.List();
            var environments = await EnvironmentService.List(null);
            return applications
                .Select(r => r.ResourceId)
                .Concat(environments.Select(r => r.ResourceId))
                .OrderBy(r => r);
        }

        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment + "/bind"), Admin]
        public async Task Bind(BindRoleResourceInput input)
        {
            await PermissionService.BindRoleResource(GetResourceId(), input.Role, input.Action);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/bind"), Admin]
        public async Task Unbind(UnbindRoleResourceInput input)
        {
            await PermissionService.UnbindRoleResource(GetResourceId(), input.Role);
        }
    }
}