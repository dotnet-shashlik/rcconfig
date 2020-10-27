using Shashlik.Kernel.Attributes;

// ReSharper disable InconsistentNaming

namespace Jinkong.RC.Config
{
    [AutoOptions("RCConfig")]
    public class RCOptions
    {
        public string Server { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string Websocket { get; set; }

        /// <summary>
        /// 轮询间隔
        /// </summary>
        public int? Polling { get; set; }
    }
}