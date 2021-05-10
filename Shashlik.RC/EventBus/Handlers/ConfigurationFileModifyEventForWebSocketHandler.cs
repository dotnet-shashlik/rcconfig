using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Common;
using Shashlik.RC.Services.ConfigurationFile;
using Shashlik.RC.WebSocket;

namespace Shashlik.RC.EventBus.Handlers
{
    /// <summary>
    /// 配置文件刷新,ws通知client
    /// </summary>
    public class ConfigurationFileModifyEventForWebSocketHandler : INotificationHandler<ConfigurationFileModifyEvent>
    {
        public ConfigurationFileModifyEventForWebSocketHandler(WebSocketContext webSocketContext, ConfigurationFileService configurationFileService,
            ILogger<ConfigurationFileModifyEventForWebSocketHandler> logger)
        {
            WebSocketContext = webSocketContext;
            ConfigurationFileService = configurationFileService;
            Logger = logger;
        }

        private WebSocketContext WebSocketContext { get; }
        private ConfigurationFileService ConfigurationFileService { get; }
        private ILogger<ConfigurationFileModifyEventForWebSocketHandler> Logger { get; }

        public async Task Handle(ConfigurationFileModifyEvent notification, CancellationToken cancellationToken)
        {
            var configurationFile = await ConfigurationFileService.Get(notification.EnvironmentResourceId, notification.FileId);
            if (configurationFile is null)
            {
                Logger.LogWarning($"file[{notification.EnvironmentResourceId}/{notification.FileId}] is not exist");
                return;
            }

            await WebSocketContext.SendAsync(notification.EnvironmentResourceId, Constants.Command.Refresh, new
            {
                File = configurationFile.Name
            }, cancellationToken);
        }
    }
}