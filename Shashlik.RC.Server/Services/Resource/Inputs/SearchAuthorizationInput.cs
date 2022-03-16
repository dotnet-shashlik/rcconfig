#nullable disable
using Shashlik.RC.Server.Common;

namespace Shashlik.RC.Server.Services.Permission.Inputs
{
    public class SearchAuthorizationInput : PageInput
    {
        public string Id { get; set; }

        public string Role { get; set; }
    }
}