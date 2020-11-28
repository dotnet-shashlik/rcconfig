namespace Shashlik.RC.Models
{
    public class ConfigSimpleModel
    {
        /// <summary>
        /// 配置id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 配置类型,json,yaml
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 配置是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
