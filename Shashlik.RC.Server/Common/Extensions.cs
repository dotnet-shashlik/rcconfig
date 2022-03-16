using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Server.Secret.GrantTypes;
using Shashlik.RC.Server.Services.Resource;
using Shashlik.RC.Server.Services.Resource.Dtos;
using Shashlik.RC.Server.Services.Permission;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Common
{
    public static class Extensions
    {
        public static IEnumerable<ResourceActionDto> CombineResourceAction(this IEnumerable<ResourceActionDto> claims)
        {
            return claims
                .GroupBy(r => r.Id)
                .Select(r =>
                    new ResourceActionDto
                    {
                        Id = r.Key,
                        Action = r.Select(v => v.Action.ParseTo<PermissionAction>()).Aggregate((a, b) => a | b)
                    }
                );
        }

        public static async Task<PageModel<T>> PageQuery<T>(this IQueryable<T> source, PageInput input)
        {
            var total = source.Count();
            if (total == 0)
                return new PageModel<T> { Rows = new List<T>() };
            var list = await source.DoPage(input.PageIndex, input.PageSize).ToListAsync();
            return new PageModel<T>
            {
                Total = total,
                Rows = list
            };
        }

        public static string ToPermissionActionString(this PermissionAction permissionAction)
        {
            return ((int)permissionAction).ToString();
        }

        public static string? GetValue(this IHeaderDictionary header, string key)
        {
            return header.TryGetValue(key, out var value) ? value.ToString() : null;
        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated ?? false)
                return null;
            return user.FindFirstValue(ClaimTypes.NameIdentifier)?.ParseTo<int>();
        }

        /// <summary>
        /// 获取策略实现
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetStrategy<T>(this IEnumerable<T> source, string strategy)
            where T : IServiceStrategy
        {
            return source.FirstOrDefault(r => r.Strategy == strategy);
        }
    }
}