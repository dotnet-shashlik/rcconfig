#nullable disable
using MediatR;

namespace Shashlik.RC.EventBus
{
    public class InnerServerNotifyEvent : INotification
    {
        public object InnerServerEvent { get; set; }
    }
}