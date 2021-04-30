namespace Shashlik.RC.Utils
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
    }
}