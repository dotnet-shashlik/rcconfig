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
        /// websocket connection url
        /// </summary>
        public string Websocket { get; set; }

        /// <summary>
        /// 轮询间隔, 单位秒
        /// </summary>
        public int? Polling { get; set; }
    }
}