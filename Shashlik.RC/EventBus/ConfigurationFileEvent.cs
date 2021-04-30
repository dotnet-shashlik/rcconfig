namespace Shashlik.RC.EventBus
{
    /// <summary>
    /// 配置文件变动事件
    /// </summary>
    public class ConfigurationFileEvent : IClusterNotifyEvent
    {
        public int FileId { get; set; }

        public int ApplicationId { get; set; }

        public int EnvironmentId { get; set; }

        public string? BeforeContent { get; set; }

        public string? AfterContent { get; set; }

        public string RequestPath => GetType().Name;
    }
}