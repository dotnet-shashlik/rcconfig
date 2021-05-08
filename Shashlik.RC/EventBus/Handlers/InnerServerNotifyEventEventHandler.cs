using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Shashlik.RC.Cluster;

namespace Shashlik.RC.EventBus.Handlers
{
    /// <summary>
    /// 内部事件,发起通知
    /// </summary>
    public class InnerServerNotifyEventEventHandler : INotificationHandler<InnerServerNotifyEvent>
    {
        public InnerServerNotifyEventEventHandler(ClusterService clusterService)
        {
            ClusterService = clusterService;
        }

        private ClusterService ClusterService { get; }

        public async Task Handle(InnerServerNotifyEvent notification, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            ClusterService.InnerNotify(notification);
        }
    }
}