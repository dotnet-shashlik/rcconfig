#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shashlik.RC.Services.Permission;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Common
{
    public class ResourceModel
    {
        public string Id { get; set; }
        public PermissionAction Action { get; set; }

        public string ActionStr => Action.ToString();

        public ResourceModel(Claim claim)
        {
            Id = claim.Type.TrimStart(PermissionService.ResourceClaimTypePrefix.ToCharArray());
            Action = claim.Value.ParseTo<PermissionAction>();
        }

        public static IEnumerable<ResourceModel> FromClaims(IEnumerable<Claim> claims)
        {
            return claims.Select(r => new ResourceModel(r));
        }
    }
}