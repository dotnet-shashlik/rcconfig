using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    }
}