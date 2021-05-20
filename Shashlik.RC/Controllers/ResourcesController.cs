using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Application;
using Shashlik.RC.Services.Environment;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Permission.Inputs;
using Shashlik.RC.Services.Resource;
using Shashlik.RC.Services.Resource.Dtos;

namespace Shashlik.RC.Controllers
{
    public class ResourcesController : ApiControllerBase
    {
        public ResourcesController(ApplicationService applicationService, EnvironmentService environmentService, ResourceService permissionService)
        {
            ApplicationService = applicationService;
            EnvironmentService = environmentService;
            PermissionService = permissionService;
        }

        private ApplicationService ApplicationService { get; }
        private EnvironmentService EnvironmentService { get; }
        private ResourceService PermissionService { get; }

        [HttpGet]
        public async Task<IEnumerable<ResourceDto>> List()
        {
            var applications = await ApplicationService.List();
            var environments = await EnvironmentService.List(null);
            return applications
                    .Select(r => new ResourceDto
                    {
                        Id = r.ResourceId
                    })
                    .Concat(environments.Select(r => new ResourceDto
                    {
                        Id = r.ResourceId
                    }))
                    .OrderBy(r => r.Id)
                ;
        }

        [HttpGet("authorizations")]
        [AllowAnonymous]
        public async Task<PageModel<ResourceAuthorizationDto>> Authorizations([FromQuery] SearchAuthorizationInput input)
        {
            return await PermissionService.SearchAuthorization(input);
        }

        [HttpPost(Constants.ResourceRoute.ApplicationAndEnvironment + "/auth"), Admin]
        public async Task Auth(AuthRoleResourceInput input)
        {
            await PermissionService.AuthorizeRoleResource(GetResourceId(), input.Role, input.Action);
        }

        [HttpDelete(Constants.ResourceRoute.ApplicationAndEnvironment + "/auth"), Admin]
        public async Task UnAuth(UnAuthRoleResourceInput input)
        {
            await PermissionService.UnAuthorizeRoleResource(GetResourceId(), input.Role);
        }
    }
}