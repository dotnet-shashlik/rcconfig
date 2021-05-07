namespace Shashlik.RC.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 数据库相关
        /// </summary>
        public class Db
        {
            public const string Sqlite = "SQLITE";
            public const string SqliteDefaultConn = "Data Source=./data/rc.db;";
            public const string MySql = "MYSQL";
            public const string Npgsql = "NPGSQL";
            public const string SqlServer = "SQLSERVER";
        }

        public class Roles
        {
            public const string Admin = "admin";
            public const string User = "user";
        }

        public class ResourceRoute
        {
            public const string ApplicationId = "appId";
            public const string EnvironmentId = "envId";
        }
    }
}