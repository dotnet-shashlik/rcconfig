using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Application;
using Shashlik.RC.Services.Application.Dtos;
using Shashlik.RC.Services.Application.Inputs;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Resource;

namespace Shashlik.RC.Controllers
{
    public class ApplicationsController : ApiControllerBase
    {
        public ApplicationsController(ApplicationService applicationService, ResourceService permissionService)
        {
            ApplicationService = applicationService;
            PermissionService = permissionService;
        }


        private ApplicationService ApplicationService { get; }
        private ResourceService PermissionService { get; }
                     
        [HttpGet]
        public async Task<List<ApplicationDto>> Get()
        {
            var list = await ApplicationService.List();
            return (await PermissionService.DoFilterIsAdminFromContext(LoginUserId!.Value, list)).ToList();
        }

        [HttpPost, Admin]
        public async Task Post(CreateApplicationInput input)
        {
            await ApplicationService.Create(input);
        }

        [HttpPatch(Constants.ResourceRoute.Application)]
        public async Task Patch(UpdateApplicationInput input)
        {
            await ApplicationService.Update(GetResourceId(), input);
        }

        [HttpDelete(Constants.ResourceRoute.Application)]
        public async Task Delete()
        {
            await ApplicationService.Delete(GetResourceId());
        }
    }
}