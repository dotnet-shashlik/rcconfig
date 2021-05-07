#nullable disable
using Shashlik.AutoMapper;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;

namespace Shashlik.RC.Services.ConfigurationFile.Dtos
{
    public class ConfigurationFileListDto : IMapFrom<ConfigurationFiles>
    {
        /// <summary>
        /// 文件id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 文件类型,yaml/json
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 环境 name
        /// </summary>
        public int EnvironmentName { get; set; }
    }
}