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
            AccessTokenLifetime = Environment.GetEnvironmentVariable("RC_ACCESS_TOKEN_LIFETIME")?.ParseTo<int>() ?? 60 * 30;
            DbType = Environment.GetEnvironmentVariable("RC_DB_TYPE") ?? Constants.Db.Sqlite;
            DbConn = Environment.GetEnvironmentVariable("RC_DB_CONN") ?? Constants.Db.SqliteDefaultConn;
            AdminUser = Environment.GetEnvironmentVariable("RC_ADMIN_USER") ?? "admin";
            AdminPassword = Environment.GetEnvironmentVariable("RC_ADMIN_PASS") ?? "Shashlik.RC.Server";
            Servers = Environment.GetEnvironmentVariable("RC_SERVERS");
            PermissionReadPolicy = Environment.GetEnvironmentVariable("RC_PERMISSION_READ_POLICY")?.ParseTo<PermissionReadPolicy>() ??
                                   PermissionReadPolicy.Token;
            ServerToken = Environment.GetEnvironmentVariable("RC_SERVER_TOKEN") ?? "Shashlik.RC.ServerToken";
        }

        /// <summary>
        /// token有效期,秒,默认半小时
        /// </summary>
        public static int AccessTokenLifetime { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public static string DbType { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string? DbConn { get; }

        /// <summary>
        /// 管理员用户名
        /// </summary>
        public static string? AdminUser { get; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        public static string? AdminPassword { get; }

        /// <summary>
        /// 集群服务器,不支持自动扩容
        /// </summary>
        public static string? Servers { get; }

        /// <summary>
        /// 是否为单机版
        /// </summary>
        public static bool IsStandalone => string.IsNullOrWhiteSpace(Servers);

        /// <summary>
        /// 权限读取策略,默认值: Token
        /// </summary>
        public static PermissionReadPolicy PermissionReadPolicy { get; }

        /// <summary>
        /// 集群环境,服务器内部安全认证token,集群环境请一定要设置
        /// </summary>
        public static string ServerToken { get; }
    }
}