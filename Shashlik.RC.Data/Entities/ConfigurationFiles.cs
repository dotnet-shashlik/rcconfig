using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Server.Data.Entities
{
    public class ConfigurationFiles 
    {
        /// <summary>
        /// 文件id
        /// </summary>
        public int Id { get; set; }

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
        /// 文件内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTime { get; set; }

        /// <summary>
        /// 环境id
        /// </summary>
        public int EnvironmentId { get; set; }

        /// <summary>
        /// 所属环境
        /// </summary>
        public Environments Environment { get; set; }

        public string EnvironmentResourceId { get; set; }

        public class Configs : IEntityTypeConfiguration<ConfigurationFiles>
        {
            public void Configure(EntityTypeBuilder<ConfigurationFiles> builder)
            {
                builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
                builder.Property(r => r.EnvironmentResourceId).HasMaxLength(255).IsRequired();
                builder.Property(r => r.Type).HasMaxLength(255).IsRequired();
                builder.Property(r => r.Desc).HasMaxLength(255);
                builder.HasIndex(r => new {r.EnvironmentResourceId, r.Name}).IsUnique();

                builder.HasOne(r => r.Environment)
                    .WithMany(r => r.Files)
                    .HasForeignKey(r => r.EnvironmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}