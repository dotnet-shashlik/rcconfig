using System;

namespace Shashlik.RC.Services.Permission
{
    [Flags]
    public enum PermissionAction
    {
        Read = 1,
        Write = 2,
        Delete = 4
    }
}