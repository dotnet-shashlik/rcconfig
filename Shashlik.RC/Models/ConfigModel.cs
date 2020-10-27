namespace Shashlik.RC.Models
{
    public class ConfigModel
    {
        /// <summary>
        /// 配置id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 环境id
        /// </summary>
        public int EnvId { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public string EnvName { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public string EnvDesc { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 配置类型,json,xml,yaml,ini....
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 配置名称是否作为配置节点
        /// </summary>
        public bool NameIsSection { get; set; }

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
