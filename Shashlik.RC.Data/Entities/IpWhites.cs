using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// ip白名单
    /// </summary>
    public class IpWhites : IEntity<int>
    {
        public int Id { get; set; }

        /// <summary>
        /// 支持通配符
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 环境变量id
        /// </summary>
        public int EnvId { get; set; }

        /// <summary>
        /// 环境变量
        /// </summary>
        public Envs Env { get; set; }
    }

    public class IpWhitesConfig : IEntityTypeConfiguration<IpWhites>
    {
        public void Configure(EntityTypeBuilder<IpWhites> builder)
        {
            builder.Property(r => r.Ip).HasMaxLength(32).IsRequired();
            builder.HasOne(r => r.Env).WithMany(r => r.IpWhites).HasForeignKey(r => r.EnvId);
        }
    }
}
