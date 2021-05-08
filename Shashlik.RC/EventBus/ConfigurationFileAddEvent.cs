#nullable disable
namespace Shashlik.RC.EventBus
{
    public class ConfigurationFileAddEvent : IInnerServerEvent
    {
        public string Name { get; set; }
    }
}