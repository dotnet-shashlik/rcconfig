#nullable disable
namespace Shashlik.RC.Server.EventBus
{
    public class ConfigurationFileModifyEvent : IInnerServerEvent
    {
        public string EnvironmentResourceId { get; set; }
        public int FileId { get; set; }
    }
}