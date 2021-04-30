using System;
using Shashlik.RC.Permission;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Common
{
    /// <summary>
    /// 系统环境变量
    /// </summary>
    public class SystemEnvironmentUtils
    {
        static SystemEnvironmentUtils()
        {
            AccessTokenLifetime = Environment.GetEnvironmentVariable("ACCESS_TOKEN_LIFETIME")?.ParseTo<int>() ?? 60 * 60 * 2;
            DbType = Environment.GetEnvironmentVariable("DB_TYPE") ?? Constants.Db.Sqlite;
            DbConn = Environment.GetEnvironmentVariable("DB_CONN") ?? Constants.Db.SqliteDefaultConn;
            AdminUser = Environment.GetEnvironmentVariable("ADMIN_USER");
            AdminPassword = Environment.GetEnvironmentVariable("ADMIN_PASS");
            Servers = Environment.GetEnvironmentVariable("SERVER");
            PermissionReadPolicy = Environment.GetEnvironmentVariable("PERMISSION_READ_POLICY")?.ParseTo<PermissionReadPolicy>() ??
                                   PermissionReadPolicy.Token;
        }

        public static int AccessTokenLifetime { get; }
        public static string DbType { get; }
        public static string? DbConn { get; }
        public static string? AdminUser { get; }
        public static string? AdminPassword { get; }
        public static string? Servers { get; }
        public static PermissionReadPolicy PermissionReadPolicy { get; }
    }
}