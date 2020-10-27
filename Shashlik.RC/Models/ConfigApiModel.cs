namespace Shashlik.RC.Models
{
    public class ConfigApiModel
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 配置类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string content { get; set; }
    }
}
