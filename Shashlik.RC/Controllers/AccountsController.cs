using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Identity.Dtos;
using Shashlik.RC.Services.Permission;
using Shashlik.Response;

namespace Shashlik.RC.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        public AccountsController(UserServices userService)
        {
            UserService = userService;
        }

        private UserServices UserService { get; }

        [HttpGet("userInfo")]
        public async Task<UserDto> UserInfo()
        {
            return await UserService.Get(LoginUserId!.Value);
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

        [HttpDelete("{userId:int:min(1)}/resources"), Admin]
        public async Task<List<Claim>> Resources(int userid, [FromServices] PermissionService permissionService)
        {
            return await permissionService.GetResourceList(userid);
        }
    }
}