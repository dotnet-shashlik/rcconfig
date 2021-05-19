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
        public string ResourceType => Id.Contains('/') ? "Application" : "Environment";
        public PermissionAction Action { get; set; }
        public string ActionStr => Action.ToString();
        public string Role { get; set; }

        public ResourceModel()
        {
        }

        public ResourceModel(Claim claim, string role = null)
        {
            Id = claim.Type.TrimStart(PermissionService.ResourceClaimTypePrefix.ToCharArray());
            Action = claim.Value.ParseTo<PermissionAction>();
            Role = null;
        }

        public static IEnumerable<ResourceModel> FromClaims(IEnumerable<Claim> claims)
        {
            return claims.Select(r => new ResourceModel(r));
        }
    }
}