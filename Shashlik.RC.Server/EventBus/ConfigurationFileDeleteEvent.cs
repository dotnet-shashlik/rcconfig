#nullable disable
namespace Shashlik.RC.Server.EventBus
{
    public class ConfigurationFileDeleteEvent : IInnerServerEvent
    {
        public string Name { get; set; }
    }
}