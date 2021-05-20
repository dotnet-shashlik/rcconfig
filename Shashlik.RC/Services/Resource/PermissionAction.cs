using System;

namespace Shashlik.RC.Services.Resource
{
    [Flags]
    public enum PermissionAction
    {
        Read = 0,
        Write = 1,
        Delete = 2
    }
}