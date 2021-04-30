using System.Collections.Generic;
using Shashlik.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Data.Entities
{
    public class Applications : IEntity<int>, IResource
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

        public List<Environments> Environments { get; set; }

        /// <summary>
        /// 资源id
        /// </summary>
        public string ResourceId => Id.ToString();

        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName => Name;

        public class Configs : IEntityTypeConfiguration<Applications>
        {
            public void Configure(EntityTypeBuilder<Applications> builder)
            {
                builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
                builder.Property(r => r.Desc).HasMaxLength(255);
            }
        }
    }
}