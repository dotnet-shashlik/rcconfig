using System;

namespace Shashlik.RC.Services.Permission
{
    /// <summary>
    /// 权限读取策略
    /// </summary>
    [Flags]
    public enum PermissionReadPolicy
    {
        /// <summary>
        /// token中读取
        /// </summary>
        Token = 1,

        /// <summary>
        /// 数据库中读取
        /// </summary>
        Db = 2
    }
}