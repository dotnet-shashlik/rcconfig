using MediatR;

namespace Shashlik.RC.Server.EventBus
{
    /// <summary>
    /// 服务器内部事件
    /// </summary>
    public interface IInnerServerEvent : INotification
    {
    }
}