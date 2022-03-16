using System;

namespace Shashlik.RC.Server.Services.Resource
{
    [Flags]
    public enum PermissionAction
    {
        Read = 1,
        Write = 2,
        Delete = 4
    }
}