using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.Permission
{
    [Scoped]
    public class PermissionService
    {
        public PermissionService(RoleService roleService, RCDbContext dbContext)
        {
            RoleService = roleService;
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }
        private RoleService RoleService { get; }

        private const string ResourceClaimTypePrefix = "RESOURCE:";

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
                .Where(s => resourceList.Any(
                    r =>
                        ResourceClaimTypePrefix + s.ResourceId == r.Type
                        && r.Value.ParseTo<PermissionAction>().HasFlag(PermissionAction.Read))
                );
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Claim>> GetResourceList(int userId)
        {
            var list = await (
                from userRole in this.DbContext.UserRoles
                join role in this.DbContext.Roles on userRole.RoleId equals role.Id
                join roleClaim in this.DbContext.RoleClaims on role.Id equals roleClaim.RoleId
                where userRole.UserId.Equals(userId)
                select new Claim(roleClaim.ClaimType, roleClaim.ClaimValue)
            ).ToListAsync();

            return list.Where(r => r.Type.StartsWith(ResourceClaimTypePrefix)).Distinct().ToList();

            // var claims = await UserService.GetClaimsAsync(new IdentityUser<int> {Id = userId});
            // return claims.Where(r => r.Type.StartsWith(ResourceClaimTypePrefix)).ToList();
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
                .Any(r => r.Type == ResourceClaimTypePrefix + resourceId && r.Value.ParseTo<PermissionAction>().HasFlag(action));
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