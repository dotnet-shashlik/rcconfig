using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shashlik.RC.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.WebSocket
{
    /// <summary>
    /// ws聊天上下文，用于保存所有在线连接
    /// </summary>
    public class WebSocketContext : IDisposable
    {
        /// <summary>
        /// 当前在线连接
        /// </summary>
        private static ConcurrentDictionary<string, List<System.Net.WebSockets.WebSocket>> OnLineSockets { get; }
            = new ConcurrentDictionary<string, List<System.Net.WebSockets.WebSocket>>();

        /// <summary>
        /// 添加一个新的在线连接
        /// </summary>
        /// <param name="env"></param>
        /// <param name="socket"></param>
        /// <param name="appId"></param>
        internal async Task AddSocket(string appId, string env, System.Net.WebSockets.WebSocket socket)
        {
            var key = $"{env}_{appId}";
            if (OnLineSockets.TryGetValue(key, out var list))
                list.Add(socket);
            else
                OnLineSockets.TryAdd(key, new List<System.Net.WebSockets.WebSocket> { socket });

            try
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = null;
                do
                {
                    await Task.Delay(10);
                    // 客户端必须保持间隔一定时间发送消息到服务端,告诉自己活着, 否则超时后直接取消,然后杀掉连接
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(TimeSpan.FromMinutes(1)).Token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        using var ms = new MemoryStream(buffer);
                        using var reader = new StreamReader(ms);
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
                while (!result.CloseStatus.HasValue);
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {

            }
            finally
            {
                try
                {
                    list?.Remove(socket);
                    await socket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    socket?.Dispose();
                    socket = null;

                }
                catch { }
            }
        }

        /// <summary>
        /// 获取指定id的连接
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        internal List<System.Net.WebSockets.WebSocket> GetSocket(string appId, string env)
        {
            var key = $"{env}_{appId}";
            return !OnLineSockets.TryGetValue(key, out var list) ? null : list;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TMsg"></typeparam>
        /// <param name="command"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendAsync(string appId, string env, string command, object data)
        {
            //转换为统一格式之后再发
            var messageToSend = new
            {
                command,
                data
            }.ToJsonWithCamelCase();

            var byteArray = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
            var connections = GetSocket(appId, env);
            if (connections.IsNullOrEmpty())
                return;

            foreach (var socket in new List<System.Net.WebSockets.WebSocket>(connections))
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.SendAsync(byteArray, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch { }
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
                        if (socket.State != WebSocketState.Closed)
                            socket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                        socket?.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}
