using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Identity.Dtos;
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
            var res = await UserService.DeleteAsync(user);
            if (!res.Succeeded)
                Logger.LogWarning($"delete user error: {res}");
        }
    }
}