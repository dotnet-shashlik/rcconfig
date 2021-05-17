using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Services.Permission;

namespace Shashlik.RC.IdentityServer
{
    public class ProfileService : ProfileService<IdentityUser<int>>
    {
        public ProfileService(UserManager<IdentityUser<int>> userManager, IUserClaimsPrincipalFactory<IdentityUser<int>> claimsFactory,
            PermissionService permissionService) : base(
            userManager, claimsFactory)
        {
            PermissionService = permissionService;
        }

        public ProfileService(UserManager<IdentityUser<int>> userManager, IUserClaimsPrincipalFactory<IdentityUser<int>> claimsFactory,
            ILogger<ProfileService<IdentityUser<int>>> logger, PermissionService permissionService) : base(userManager, claimsFactory, logger)
        {
            PermissionService = permissionService;
        }

        private PermissionService PermissionService { get; }

        protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, IdentityUser<int> user)
        {
            // 将角色的资源数据写入token
            var claims = await PermissionService.GetDbResourceClaims(user.Id);
            context.AddRequestedClaims(claims);
            await base.GetProfileDataAsync(context, user);
        }
    }
}