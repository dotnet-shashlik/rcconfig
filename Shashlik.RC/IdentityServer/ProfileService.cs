using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Resource;

namespace Shashlik.RC.IdentityServer
{
    public class ProfileService : ProfileService<IdentityUser<int>>
    {
        public ProfileService(UserManager<IdentityUser<int>> userManager, IUserClaimsPrincipalFactory<IdentityUser<int>> claimsFactory,
            ResourceService permissionService) : base(
            userManager, claimsFactory)
        {
            PermissionService = permissionService;
        }

        public ProfileService(UserManager<IdentityUser<int>> userManager, IUserClaimsPrincipalFactory<IdentityUser<int>> claimsFactory,
            ILogger<ProfileService<IdentityUser<int>>> logger, ResourceService permissionService) : base(userManager, claimsFactory, logger)
        {
            PermissionService = permissionService;
        }

        private ResourceService PermissionService { get; }

        protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, IdentityUser<int> user)
        {
            await base.GetProfileDataAsync(context, user);
            // 将角色的资源数据写入token
            var actions = await PermissionService.GetResourceActionsByUserId(user.Id);
            var claims = actions
                .Select(r => new Claim(ResourceService.GetClaimTypeFromResourceId(r.Id), r.Action.ToPermissionActionString()));
            context.AddRequestedClaims(claims);
        }
    }
}