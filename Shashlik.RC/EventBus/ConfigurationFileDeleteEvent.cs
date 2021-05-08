#nullable disable
namespace Shashlik.RC.EventBus
{
    public class ConfigurationFileDeleteEvent : IInnerServerEvent
    {
        public string Name { get; set; }
    }
}