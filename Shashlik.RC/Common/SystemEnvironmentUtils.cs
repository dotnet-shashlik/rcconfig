using System;
using Shashlik.RC.Services.Permission;
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
            AccessTokenLifetime = Environment.GetEnvironmentVariable("RC_ACCESS_TOKEN_LIFETIME")?.ParseTo<int>() ?? 60 * 60 * 2;
            DbType = Environment.GetEnvironmentVariable("RC_DB_TYPE") ?? Constants.Db.Sqlite;
            DbConn = Environment.GetEnvironmentVariable("RC_DB_CONN") ?? Constants.Db.SqliteDefaultConn;
            AdminUser = Environment.GetEnvironmentVariable("RC_ADMIN_USER");
            AdminPassword = Environment.GetEnvironmentVariable("RC_ADMIN_PASS");
            Servers = Environment.GetEnvironmentVariable("RC_SERVER");
            PermissionReadPolicy = Environment.GetEnvironmentVariable("RC_PERMISSION_READ_POLICY")?.ParseTo<PermissionReadPolicy>() ??
                                   PermissionReadPolicy.Token;
            ServerToken = Environment.GetEnvironmentVariable("RC_SERVER_TOKEN") ?? string.Empty;
        }

        public static int AccessTokenLifetime { get; }
        public static string DbType { get; }
        public static string? DbConn { get; }
        public static string? AdminUser { get; }
        public static string? AdminPassword { get; }
        public static string? Servers { get; }
        public static PermissionReadPolicy PermissionReadPolicy { get; }
        public static string ServerToken { get; }
    }
}