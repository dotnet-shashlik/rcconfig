using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Shashlik.RC.EventBus
{
    /// <summary>
    /// 配置文件变动事件,通知集群
    /// </summary>
    public class ClusterNotifyEventHandler : INotificationHandler<IClusterNotifyEvent>
    {
        public Task Handle(IClusterNotifyEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}