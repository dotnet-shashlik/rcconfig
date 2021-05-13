using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Identity.Dtos;
using Shashlik.RC.Services.Identity.Inputs;
using Shashlik.RC.Services.Permission;
using Shashlik.Response;

namespace Shashlik.RC.Controllers
{
    public class UsersController : ApiControllerBase
    {
        public UsersController(UserService userService)
        {
            UserService = userService;
        }

        private UserService UserService { get; }

        [HttpGet("current")]
        public async Task<UserDetailDto> UserInfo()
        {
            return (await UserService.Get(LoginUserId!.Value))!;
        }

        [HttpPatch("password")]
        public async Task ChangePassword(ChangePasswordInput input)
        {
            var user = await UserService.FindByIdAsync(LoginUserId.ToString());
            var res = await UserService.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!res.Succeeded)
                throw ResponseException.ArgError(res.ToString());
        }

        [HttpGet, Admin]
        public async Task<List<UserDto>> Get()
        {
            return await UserService.Get();
        }

        [HttpDelete("{userId:int:min(1)}"), Admin]
        public async Task Delete(int userId)
        {
            var user = await UserService.FindByIdAsync(userId.ToString());
            if (user is null)
                throw ResponseException.NotFound();
            if (await UserService.IsInRoleAsync(user, Constants.Roles.Admin))
                throw ResponseException.Forbidden("管理员不可删除");

            var res = await UserService.DeleteAsync(user);
            if (!res.Succeeded)
                Logger.LogWarning($"delete user error: {res}");
        }

        [HttpGet("{userId:int:min(1)}/resources"), Admin]
        public async Task<IEnumerable<Claim>> Resources(int userid, [FromServices] PermissionService permissionService)
        {
            return await permissionService.GetDbResourceList(userid);
        }
    }
}