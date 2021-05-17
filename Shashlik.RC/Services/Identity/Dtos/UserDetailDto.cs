#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Shashlik.RC.Common;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Services.Identity.Dtos
{
    public class UserDetailDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string NickName => Claims.FirstOrDefault(r => r.Type == Constants.UserClaimTypes.NickName)?.Value;
        public string Remark => Claims.FirstOrDefault(r => r.Type == Constants.UserClaimTypes.Remark)?.Value;

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public IEnumerable<Claim> Claims { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public string RolesStr => Roles.Join(",");
    }
}