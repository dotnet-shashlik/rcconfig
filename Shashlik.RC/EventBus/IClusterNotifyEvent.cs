using MediatR;

namespace Shashlik.RC.EventBus
{
    /// <summary>
    /// 集群通知事件
    /// </summary>
    public interface IClusterNotifyEvent : INotification
    {
        /// <summary>
        /// 请求路径
        /// </summary>
        string RequestPath { get; }
    }
}