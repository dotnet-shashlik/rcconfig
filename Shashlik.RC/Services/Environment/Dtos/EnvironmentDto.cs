#nullable disable
using System.Collections.Generic;
using Shashlik.AutoMapper;
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;

namespace Shashlik.RC.Services.Environment.Dtos
{
    public class EnvironmentDto : IMapFrom<Environments>, IResource
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// ip白名单
        /// </summary>
        public string IpWhites { get; set; }

        /// <summary>
        /// 应用id
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// 应用name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        public List<SecretDto> Secrets { get; set; }

        /// <summary>
        /// 资源id
        /// </summary>
        public string ResourceId => $"{ApplicationName}/{Name}";
    }
}