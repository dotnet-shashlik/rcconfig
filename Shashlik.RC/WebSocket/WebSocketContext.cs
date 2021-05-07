using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shashlik.Utils.Extensions;
using WS = System.Net.WebSockets.WebSocket;

namespace Shashlik.RC.WebSocket
{
    /// <summary>
    /// ws聊天上下文，用于保存所有在线连接
    /// </summary>
    public class WebSocketContext : IDisposable
    {
        public WebSocketContext(ILogger<WebSocketContext> logger)
        {
            Logger = logger;
        }

        private ILogger<WebSocketContext> Logger { get; }

        /// <summary>
        /// 当前在线连接
        /// </summary>
        private static ConcurrentDictionary<int, List<WS>> OnLineSockets { get; } = new();

        /// <summary>
        /// 添加一个新的在线连接
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="socket"></param>
        internal async Task AddSocket(int environmentId, WS socket)
        {
            var key = environmentId;
            if (OnLineSockets.TryGetValue(key, out var list))
                list.Add(socket);
            else
                OnLineSockets.TryAdd(key, new List<WS> {socket});

            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result;
                do
                {
                    await Task.Delay(10);
                    // heart beat
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // ignore, 发送的什么内容都不管
                    }
                    else
                        break;
                } while (!result.CloseStatus.HasValue);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (WebSocketException)
            {
                // ignore
            }
            finally
            {
                try
                {
                    list?.Remove(socket);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    socket.Dispose();
                }
                catch (Exception e)
                {
                    Logger.LogDebug(e, $"dispose websocket instance occured error");
                }
            }
        }

        /// <summary>
        /// 获取指定id的连接
        /// </summary>
        /// <param name="environmentId"></param>
        /// <returns></returns>
        internal List<WS>? GetSocket(int environmentId)
        {
            return OnLineSockets.TryGetValue(environmentId, out var list) ? null : list;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="environmentId"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(int environmentId, string command, object data)
        {
            //转换为统一格式之后再发
            var messageToSend = new
            {
                command,
                data
            }.ToJsonWithCamelCase();

            var byteArray = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
            var connections = GetSocket(environmentId);
            if (connections.IsNullOrEmpty())
                return;

            foreach (var socket in new List<WS>(connections!))
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.SendAsync(byteArray, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        Logger.LogDebug(e, $"send websocket message to client occured error");
                    }
                }
            }
        }


        public void Dispose()
        {
            foreach (var item in OnLineSockets)
            {
                foreach (var socket in item.Value)
                {
                    try
                    {
                        if (socket != null)
                        {
                            if (socket.State != WebSocketState.Closed)
                                socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                            socket.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogDebug(e, $"dispose websocket instance occured error");
                    }
                }
            }
        }
    }
}