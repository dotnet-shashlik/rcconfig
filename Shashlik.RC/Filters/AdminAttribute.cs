using Microsoft.AspNetCore.Authorization;
using Shashlik.RC.Common;

namespace Shashlik.RC.Filters
{
    public class AdminAttribute : AuthorizeAttribute
    {
        public AdminAttribute()
        {
            Roles = Constants.Roles.Admin;
        }
    }
}