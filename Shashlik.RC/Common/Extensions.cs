using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Services.Permission;
using Shashlik.RC.Services.Resource;
using Shashlik.RC.Services.Resource.Dtos;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Common
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
                return new PageModel<T> {Rows = new List<T>()};
            var list = await source.DoPage(input.PageIndex, input.PageSize).ToListAsync();
            return new PageModel<T>
            {
                Total = total,
                Rows = list
            };
        }

        public static string ToPermissionActionString(this PermissionAction permissionAction)
        {
            return ((int) permissionAction).ToString();
        }
    }
}