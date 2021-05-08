#nullable disable
namespace Shashlik.RC.EventBus
{
    public class ConfigurationFileModifyEvent : IInnerServerEvent
    {
        public string Name { get; set; }
    }
}