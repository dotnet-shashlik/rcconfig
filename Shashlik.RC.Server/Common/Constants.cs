namespace Shashlik.RC.Server.Common;

/// <summary>
/// 常量
/// </summary>
public class Constants
{
    public class Roles
    {
        public const string Admin = "admin";
        public const string User = "user";
    }

    public class ResourceRoute
    {
        public const string Application = "{app:minlength(1):maxlength(32)}";
        public const string ApplicationKey = "app";
        public const string EnvironmentKey = "env";
        public const string ApplicationAndEnvironment = "{app:minlength(1):maxlength(32)}/{env:minlength(1):maxlength(32)}";
    }

    public class HeaderKeys
    {
        public const string ServerToken = "SERVER_TOKEN";
        public const string EventType = "EVENT_TYPE";
    }

    public class Command
    {
        /// <summary>
        /// 文件刷新
        /// </summary>
        public const string Refresh = "REFRESH";

        /// <summary>
        /// 新增文件
        /// </summary>
        public const string Add = "ADD";

        /// <summary>
        /// 删除文件
        /// </summary>
        public const string Delete = "DELETE";
    }

    public class UserClaimTypes
    {
        public const string NickName = "nickName";
        public const string Remark = "remark";
    }

    public class Regexs
    {
        public const string Name = "^[0-9a-zA-Z_]{1,32}$";
    }

    /// <summary>
    /// 事件
    /// </summary>
    public class Events
    {
        /// <summary>
        /// 资源更新
        /// </summary>
        public const string ResourceUpdated = "RESOURCE_UPDATED";
    }
}