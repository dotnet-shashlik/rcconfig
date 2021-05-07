using System;

namespace Shashlik.RC.Services.Permission
{
    [Flags]
    public enum PermissionAction
    {
        Read = 0,
        Write = 1,
        Delete = 2
    }
}