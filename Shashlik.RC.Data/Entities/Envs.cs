using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// 环境变量
    /// </summary>
    public class Envs : IEntity<int>
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 所属app
        /// </summary>
        public Apps App { get; set; }

        /// <summary>
        /// app secretKey
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 环境变量名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 下属配置
        /// </summary>
        public List<Configs> Configs { get; set; }

        /// <summary>
        /// ip白名单
        /// </summary>
        public List<IpWhites> IpWhites { get; set; }
    }

    public class EnvsConfig : IEntityTypeConfiguration<Envs>
    {
        public void Configure(EntityTypeBuilder<Envs> builder)
        {
            builder.Property(r => r.AppId).HasMaxLength(32).IsRequired();
            builder.HasOne(r => r.App).WithMany(r => r.Envs).HasForeignKey(r => r.AppId);
            builder.Property(r => r.Name).HasMaxLength(32).IsRequired();
            builder.HasIndex(r => new { r.AppId, r.Name }).IsUnique();
            builder.Property(r => r.Desc).HasMaxLength(32);
            builder.Property(r => r.Key).HasMaxLength(64).IsRequired();
        }
    }
}
