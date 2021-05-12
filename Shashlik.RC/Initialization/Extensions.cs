using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel;
using Shashlik.RC.Common;
using Shashlik.RC.Data;
using Shashlik.RC.Services.Identity;

namespace Shashlik.RC.Initialization
{
    public static class Extensions
    {
        public static IKernelServiceProvider MigrationDb(this IKernelServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<RCDbContext>();
            dbContext.Database.Migrate();
            return serviceProvider;
        }

        private static void InitRole(RoleService roleService, string role, ILogger logger)
        {
            var adminRole = roleService.FindByNameAsync(role).GetAwaiter().GetResult();
            if (adminRole is null)
            {
                try
                {
                    var res = roleService.CreateAsync(new IdentityRole<int>
                    {
                        Name = role
                    }).GetAwaiter().GetResult();
                    if (!res.Succeeded && res.Errors.All(r => r.Code != "DuplicateRoleName"))
                        throw new InvalidOperationException($"create role \"{role}\" failed, detail: {res}");
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, $"create role \"{role}\" occurred error");
                }
            }
        }

        public static IKernelServiceProvider InitRoleAndAdminUser(this IKernelServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Initialization");
            using var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            using var roleService = scope.ServiceProvider.GetRequiredService<RoleService>();
            InitRole(roleService, Constants.Roles.Admin, logger);
            InitRole(roleService, Constants.Roles.User, logger);

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
                    if (!res.Succeeded && res.Errors.All(r => r.Code != "DuplicateUserName"))
                        throw new InvalidOperationException($"create admin user failed, detail: {res}");
                    userService.AddClaimsAsync(user, new List<Claim>()
                    {
                        new Claim(Constants.UserClaimTypes.NickName, "超级管理员")
                    }).GetAwaiter().GetResult();
                    userService.AddToRoleAsync(user, Constants.Roles.Admin).GetAwaiter().GetResult();
                }
                catch (DbUpdateException e)
                {
                    logger.LogWarning(e, $"create admin user occurred error");
                }
            }

            return serviceProvider;
        }
    }
}