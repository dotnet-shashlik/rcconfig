using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Data;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Identity;
using Shashlik.RC.Server.Services.Permission.Inputs;
using Shashlik.RC.Server.Services.Resource.Dtos;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.Resource
{
    [Scoped]
    public class ResourceService
    {
        public ResourceService(RoleService roleService, RCDbContext dbContext, IHttpContextAccessor httpContextAccessor,
            UserService userService)
        {
            RoleService = roleService;
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
            UserService = userService;
        }

        private RCDbContext DbContext { get; }
        private RoleService RoleService { get; }
        private UserService UserService { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }

        public const string ResourceClaimTypePrefix = "RESOURCE:";

        public static string GetResourceIdFromClaimType(string claimType)
        {
            return claimType.TrimStart(ResourceClaimTypePrefix.ToCharArray());
        }

        public static string GetResourceIdFromClaimType(Claim claim)
        {
            return claim.Type.TrimStart(ResourceClaimTypePrefix.ToCharArray());
        }

        public static ResourceActionDto GetResourceActionByClaim(Claim claim)
        {
            return new()
            {
                Id = GetResourceIdFromClaimType(claim),
                Action = claim.Value.ParseTo<PermissionAction>()
            };
        }

        public static string GetClaimTypeFromResourceId(string resourceId)
        {
            return ResourceClaimTypePrefix + resourceId;
        }

        /// <summary>
        /// 过滤没有读取权限的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isAdmin"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> DoFilter<T>(int userId, bool isAdmin, IEnumerable<T> source)
            where T : IResource
        {
            if (isAdmin)
                return source;
            var resourceList = await GetResourceList(userId);
            return source
                .Where(s => resourceList.Any(r => s.ResourceId == r.Id && r.Action.HasFlag(PermissionAction.Read)));
        }

        /// <summary>
        /// 过滤没有读取权限的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> DoFilterIsAdminFromContext<T>(int userId, IEnumerable<T> source)
            where T : IResource
        {
            return await DoFilter(userId, HttpContextAccessor.HttpContext!.User.IsInRole(Constants.Roles.Admin), source);
        }

        /// <summary>
        /// 过滤没有读取权限的数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> DoFilterIsAdminFromDb<T>(int userId, IEnumerable<T> source)
            where T : IResource
        {
            var user = new IdentityUser<int>()
            {
                Id = userId
            };

            return await DoFilter(userId, await UserService.IsInRoleAsync(user, Constants.Roles.Admin), source);
        }

        public async Task<IEnumerable<ResourceActionDto>> GetResourceList(int userId)
        {
            return await GetResourceActionsByUserId(userId);
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ResourceActionDto>> GetResourceActionsByUserId(int userId)
        {
            var list = await (
                    from userRole in DbContext.UserRoles
                    join role in DbContext.Roles on userRole.RoleId equals role.Id
                    join roleClaim in DbContext.RoleClaims on role.Id equals roleClaim.RoleId
                    where userRole.UserId.Equals(userId)
                    select new { roleClaim.ClaimType, roleClaim.ClaimValue }
                )
                .Distinct()
                .ToListAsync();

            return list
                    .Where(r => r.ClaimType.StartsWith(ResourceClaimTypePrefix))
                    .Select(r => new ResourceActionDto
                    {
                        Id = GetResourceIdFromClaimType(r.ClaimType),
                        Action = r.ClaimValue.ParseTo<PermissionAction>()
                    })
                    .CombineResourceAction()
                ;
        }

        /// <summary>
        /// 获取资源列表
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ResourceActionDto>> GetResourceActionsByRole(string role)
        {
            var roleEntity = await RoleService.FindByNameAsync(role);
            if (roleEntity is null)
                throw ResponseException.NotFound();

            return (await RoleService.GetClaimsAsync(roleEntity))
                .Where(r => r.Type.StartsWith(ResourceService.ResourceClaimTypePrefix))
                .Select(r => new ResourceActionDto
                {
                    Id = r.Type,
                    Action = r.Value.ParseTo<PermissionAction>()
                });
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
        /// <param name="resourceId"></param>
        /// <param name="role"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task AuthorizeRoleResource(string resourceId, string role, PermissionAction action)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();
            var identityRole = await RoleService.FindByNameAsync(role);
            var claims = await RoleService.GetClaimsAsync(identityRole);
            var claim = claims.SingleOrDefault(r => r.Type == GetClaimTypeFromResourceId(resourceId));
            IdentityResult res;
            if (claim is not null)
            {
                action |= claim.Value.ParseTo<PermissionAction>();
                res = await RoleService.RemoveClaimAsync(identityRole, claim);
                if (!res.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw ResponseException.ArgError(res.ToString());
                }
            }

            res = await RoleService.AddClaimAsync(identityRole,
                new Claim(GetClaimTypeFromResourceId(resourceId), action.ToPermissionActionString()));
            if (!res.Succeeded)
            {
                await transaction.RollbackAsync();
                throw ResponseException.ArgError(res.ToString());
            }

            await transaction.CommitAsync();
        }

        /// <summary>
        /// 解绑角色与资源
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task UnAuthorizeRoleResource(string resourceId, string role)
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync();

            var identityRole = await RoleService.FindByNameAsync(role);
            var claims = await RoleService.GetClaimsAsync(identityRole);
            var delClaims = claims.Where(r => r.Type == GetClaimTypeFromResourceId(resourceId)).ToList();
            if (delClaims.IsNullOrEmpty())
                throw ResponseException.NotFound();

            foreach (var item in delClaims)
            {
                var res = await RoleService.RemoveClaimAsync(identityRole, item);
                if (!res.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw ResponseException.ArgError(res.ToString());
                }
            }

            await transaction.CommitAsync();
        }

        /// <summary>
        /// 所有已授权数据
        /// </summary>
        /// <returns></returns>
        public async Task<PageModel<ResourceAuthorizationDto>> SearchAuthorization(SearchAuthorizationInput input)
        {
            var id = input.Id.IsNullOrWhiteSpace() ? null : GetClaimTypeFromResourceId(input.Id);
            var roles = await DbContext
                .Set<IdentityRole<int>>()
                .WhereIf(!input.Role.IsNullOrWhiteSpace(), r => r.Name == input.Role)
                .ToListAsync();

            int? roleId = null;
            if (!input.Role.IsNullOrWhiteSpace())
            {
                roleId = roles.SingleOrDefault(r => r.Name == input.Role)?.Id;
                if (!roleId.HasValue)
                    throw ResponseException.ArgError("角色参数错误");
            }

            var claims = await DbContext
                .Set<IdentityRoleClaim<int>>()
                .WhereIf(roleId.HasValue, r => r.RoleId == roleId)
                .WhereIf(!id.IsNullOrWhiteSpace(), r => r.ClaimType == id)
                .ToListAsync();
            var total = claims.Count;

            var list = (
                    from claim in claims
                    join role in roles on claim.RoleId equals role.Id
                    orderby claim.ClaimType
                    select new ResourceAuthorizationDto
                    {
                        Id = claim.ClaimType.TrimStart(ResourceClaimTypePrefix.ToCharArray()),
                        Action = claim.ClaimValue.ParseTo<PermissionAction>(),
                        Role = role.Name
                    }
                )
                .Skip((input.PageIndex - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToList();

            return new PageModel<ResourceAuthorizationDto>(total, list);
        }
    }
}