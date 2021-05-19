#nullable disable
using Shashlik.RC.Common;

namespace Shashlik.RC.Services.Permission.Inputs
{
    public class SearchAuthorizationInput : PageInput
    {
        public string Id { get; set; }

        public string Role { get; set; }
    }
}