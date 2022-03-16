#nullable disable
namespace Shashlik.RC.Server.EventBus
{
    public class ConfigurationFileAddEvent : IInnerServerEvent
    {
        public string Name { get; set; }
    }
}