using Microsoft.AspNetCore.Authorization;
using Shashlik.RC.Server.Common;

namespace Shashlik.RC.Server.Filters
{
    public class AdminAttribute : AuthorizeAttribute
    {
        public AdminAttribute()
        {
            Roles = Constants.Roles.Admin;
        }
    }
}