#nullable disable
using MediatR;

namespace Shashlik.RC.Server.EventBus
{
    public class InnerServerNotifyEvent : INotification
    {
        public object InnerServerEvent { get; set; }
    }
} 