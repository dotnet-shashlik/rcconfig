using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Identity;
using Shashlik.RC.Server.Services.Identity.Dtos;

namespace Shashlik.RC.Server.Secret.GrantTypes;

[Scoped]
public class PasswordGrantType : IGrantType
{
    public PasswordGrantType(UserService userService)
    {
        UserService = userService;
    }

    public string Strategy => "password";

    private UserService UserService { get; }

    public async Task<UserDetailDto?> Get(JObject request)
    {
        try
        {
            var username = request.Value<string>("username");
            var password = request.Value<string>("password");
            if (username is null)
                throw ResponseException.ArgError("用户名不能为空");
            if (password is null)
                throw ResponseException.ArgError("密码不能为空");

            var user = await UserService.FindByNameAsync(username);
            if (user is null)
                throw ResponseException.ArgError("用户名或密码错误");
            if (!await UserService.CheckPasswordAsync(user, password))
                throw ResponseException.ArgError("用户名或密码错误");

            return await UserService.Get(user.Id);
        }
        catch (Exception e) when (e is not ResponseException)
        {
            throw ResponseException.ArgError("参数错误", debug: e.ToString());
        }
    }
}