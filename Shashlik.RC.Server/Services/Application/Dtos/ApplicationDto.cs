#nullable disable
using Shashlik.AutoMapper;
using Shashlik.RC.Server.Data;
using Shashlik.RC.Server.Data.Entities;

namespace Shashlik.RC.Server.Services.Application.Dtos
{
    public class ApplicationDto : IMapFrom<Applications>, IResource
    {
        public int Id { get; set; }

        /// <summary>
        /// app名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// app描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 资源id
        /// </summary>
        public string ResourceId { get; set; }
    }
}