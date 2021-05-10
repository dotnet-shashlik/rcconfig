using System;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Identity;

namespace Shashlik.RC.Initialization
{
    public static class Extensions
    {
        private static void CreateRole(RoleService roleService, string role)
        {
            var adminRole = roleService.FindByNameAsync(role).GetAwaiter().GetResult();
            if (adminRole is null)
            {
                var res = roleService.CreateAsync(new IdentityRole<int>
                {
                    Name = role
                }).GetAwaiter().GetResult();
                if (!res.Succeeded && res.Errors.All(r => r.Code != "DuplicateRoleName"))
                    throw new InvalidOperationException($"create role \"{role}\" failed, detail: {res}");
            }
        }


        public static IKernelServiceProvider InitRoleAndAdminUser(this IKernelServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            using var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            using var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
            CreateRole(roleService, Constants.Roles.Admin);
            CreateRole(roleService, Constants.Roles.User);

            var user = userService.FindByNameAsync(SystemEnvironmentUtils.AdminUser).GetAwaiter().GetResult();
            if (user is null)
            {
                user = new IdentityUser<int>
                {
                    UserName = SystemEnvironmentUtils.AdminUser
                };


                var res = userService.CreateAsync(user, SystemEnvironmentUtils.AdminPassword).GetAwaiter().GetResult();
                if (!res.Succeeded && res.Errors.All(r => r.Code != "DuplicateUserName"))
                    throw new InvalidOperationException($"create admin user failed, detail: {res}");
                userService.AddToRoleAsync(user, Constants.Roles.Admin).GetAwaiter().GetResult();
            }

            return serviceProvider;
        }
    }
}