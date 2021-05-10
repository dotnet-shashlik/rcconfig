#nullable disable
namespace Shashlik.RC.EventBus
{
    public class ConfigurationFileModifyEvent : IInnerServerEvent
    {
        public string EnvironmentResourceId { get; set; }
        public int FileId { get; set; }
    }
}