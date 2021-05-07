using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    public class Environments : IEntity<int>, IResource
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
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        public Applications Application { get; set; }

        public List<ConfigurationFiles> Files { get; set; }

        public List<Secrets> Secrets { get; set; }

        /// <summary>
        /// 资源id
        /// </summary>
        public string ResourceId => $"{Application.Name}/{Name}";

        public class Configs : IEntityTypeConfiguration<Environments>
        {
            public void Configure(EntityTypeBuilder<Environments> builder)
            {
                builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
                builder.HasIndex(r => r.Name).IsUnique();
                builder.Property(r => r.IpWhites).HasMaxLength(255);
                builder.Property(r => r.Desc).HasMaxLength(255);

                builder.HasOne(r => r.Application)
                    .WithMany(r => r.Environments)
                    .HasForeignKey(r => r.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    ;
            }
        }
    }
}