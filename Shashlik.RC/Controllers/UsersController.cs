using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Common;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Identity.Dtos;
using Shashlik.RC.Services.Identity.Inputs;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Resource;
using Shashlik.RC.Services.Resource.Dtos;

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

        [HttpGet("{userId:int:min(1)}"), Admin]
        public async Task<UserDetailDto> Get(int userId)
        {
            var user = await UserService.Get(userId);
            return user ?? throw ResponseException.NotFound();
        }

        [HttpPost, Admin]
        public async Task Create(CreateUserInput input)
        {
            if (input.Roles.Contains(Constants.Roles.Admin))
                throw ResponseException.Forbidden("不允许创建管理员");
            await UserService.CreateUser(input);
        }

        [HttpPatch("{userId:int:min(1)}"), Admin]
        public async Task Update(int userId, UpdateUserInput input)
        {
            if (input.Roles.Contains(Constants.Roles.Admin) ||
                await UserService.IsInRoleAsync(new IdentityUser<int>() {Id = userId}, Constants.Roles.Admin))
                throw ResponseException.Forbidden("不允许编辑管理员");
            await UserService.Update(userId, input);
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
        public async Task<IEnumerable<ResourceActionDto>> Resources(int userid, [FromServices] ResourceService permissionService)
        {
           return await permissionService.GetResourceActionsByUserId(userid);
        }
    }
}