namespace Shashlik.RC.Config
{
    public class RCOptions
    {
        /// <summary>
        /// 服务 api获取地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// appId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// app key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// get configuration api url 
        /// </summary>
        public string ApiUrl => $"{Server.TrimEnd('/')}/config/get";

        /// <summary>
        /// websocket connection url
        /// </summary>
        public string Websocket => $"{Server.ToLower().Replace("http://", "ws://").Replace("https://", "wss://").TrimEnd('/')}/ws/subscribe";

        /// <summary>
        /// 轮询间隔, 单位秒, 0:不轮询
        /// </summary>
        public int Polling { get; set; } = 0;
    }
}