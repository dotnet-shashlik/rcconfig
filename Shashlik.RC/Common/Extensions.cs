using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.RC.Services.Permission;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Common
{
    public static class Extensions
    {
        public static IEnumerable<Claim> CombineResource(this IEnumerable<Claim> claims)
        {
            return claims
                .GroupBy(r => r.Type)
                .Select(r =>
                    new Claim(r.Key,
                        r.Select(v => v.ParseTo<PermissionAction>()).Aggregate((a, b) => a | b).ToString())
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
    }
}