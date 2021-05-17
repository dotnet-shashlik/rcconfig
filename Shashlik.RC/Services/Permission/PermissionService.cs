using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Common;
using Shashlik.RC.Data;
using Shashlik.RC.Services.Identity;

namespace Shashlik.RC.Services.Permission
{
    [Scoped]
    public class PermissionService
    {
        public PermissionService(RoleService roleService, RCDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            RoleService = roleService;
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
        }

        private RCDbContext DbContext { get; }
        private RoleService RoleService { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        private IEnumerable<Claim> RequestUserClaims => HttpContextAccessor.HttpContext?.User.Claims ?? new List<Claim>();

        public const string ResourceClaimTypePrefix = "RESOURCE:";

        /// <summary>
        /// 过滤没有读取权限的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> DoFilter<T>(int userId, IEnumerable<T> source)
            where T : IResource
        {
            var resourceList = await GetResourceList(userId);
            return source
                .Where(s => resourceList.Any(r => s.ResourceId == r.Id && r.Action.HasFlag(PermissionAction.Read)));
        }

        public async Task<IEnumerable<ResourceModel>> GetResourceList(int userId)
        {
            return SystemEnvironmentUtils.PermissionReadPolicy switch
            {
                PermissionReadPolicy.Db => await GetDbResourceList(userId),
                PermissionReadPolicy.Token => RequestUserClaims.ToResources(),
                _ => throw new IndexOutOfRangeException()
            };
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ResourceModel>> GetDbResourceList(int userId)
        {
            return (await GetDbResourceClaims(userId)).ToResources();
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Claim>> GetDbResourceClaims(int userId)
        {
            var list = await (
                    from userRole in DbContext.UserRoles
                    join role in DbContext.Roles on userRole.RoleId equals role.Id
                    join roleClaim in DbContext.RoleClaims on role.Id equals roleClaim.RoleId
                    where userRole.UserId.Equals(userId)
                    select new {roleClaim.ClaimType, roleClaim.ClaimValue}
                )
                .Distinct()
                .ToListAsync();

            return list
                .Where(r => r.ClaimType.StartsWith(ResourceClaimTypePrefix))
                .Select(r => new Claim(r.ClaimType, r.ClaimValue))
                .CombineResource()
                .ToList();
        }

        /// <summary>
        /// 是否拥有操作权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resourceId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<bool> HasPermission(int userId, string resourceId, PermissionAction action)
        {
            var resourceList = await GetResourceList(userId);
            return resourceList
                .Any(r => r.Id == resourceId && r.Action.HasFlag(action));
        }

        /// <summary>
        /// 绑定角色与资源
        /// </summary>
        /// <param name="role"></param>
        /// <param name="resourceId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task BindRoleResource(string role, string resourceId, int action)
        {
            var identityRole = await RoleService.FindByNameAsync(role);
            var claims = await RoleService.GetClaimsAsync(identityRole);
            var claim = claims.FirstOrDefault(r => r.Type == ResourceClaimTypePrefix + resourceId);
            if (claim is not null)
                await RoleService.RemoveClaimAsync(identityRole, claim);
            await RoleService.AddClaimAsync(identityRole, new Claim(ResourceClaimTypePrefix + resourceId, action.ToString()));
        }
    }
}