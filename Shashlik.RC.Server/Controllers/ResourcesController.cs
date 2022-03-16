using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Application;
using Shashlik.RC.Server.Services.Environment;
using Shashlik.RC.Server.Services.Permission.Inputs;
using Shashlik.RC.Server.Services.Resource;
using Shashlik.RC.Server.Services.Resource.Dtos;
using Shashlik.RC.Server.Services.Resource.Inputs;

namespace Shashlik.RC.Server.Controllers
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
            var list = applications
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

            return await PermissionService.DoFilter(LoginUserId!.Value, User.IsInRole(Constants.Roles.Admin), list);
        }

        [HttpGet("authorizations"), Admin]
        public async Task<PageModel<ResourceAuthorizationDto>> Authorizations([FromQuery] SearchAuthorizationInput input)
        {
            return await PermissionService.SearchAuthorization(input);
        }

        [HttpPost("auth"), Admin]
        public async Task Auth(AuthRoleResourceInput input)
        {
            await PermissionService.AuthorizeRoleResource(input.ResourceId, input.Role, input.Action);
        }

        [HttpDelete("auth"), Admin]
        public async Task UnAuth(UnAuthRoleResourceInput input)
        {
            await PermissionService.UnAuthorizeRoleResource(input.ResourceId, input.Role);
        }
    }
}