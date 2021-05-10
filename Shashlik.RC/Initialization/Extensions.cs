using System;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Identity;

namespace Shashlik.RC.Initialization
{
    public static class Extensions
    {
        public static IServiceProvider InitRootUser(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            var user = userService.FindByNameAsync(SystemEnvironmentUtils.AdminUser).GetAwaiter().GetResult();
            if (user is null)
            {
                user = new IdentityUser<int>
                {
                    UserName = SystemEnvironmentUtils.AdminUser
                };

                try
                {
                    var res = userService.CreateAsync(user, SystemEnvironmentUtils.AdminPassword).GetAwaiter().GetResult();
                    if (!res.Succeeded)
                        throw new InvalidOperationException($"create admin user failed, detail: {res}");
                }
                catch (DBConcurrencyException)
                {
                    // ignore
                }
            }

            return serviceProvider;
        }
    }
}