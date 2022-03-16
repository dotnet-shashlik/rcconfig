using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Server.Data.Entities
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class Logs
    {
        public long Id { get; set; }

        public long LogTime { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string LogType { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string ResourceId { get; set; }

        public string BeforeContent { get; set; }

        public string AfterContent { get; set; }

        public class Configs : IEntityTypeConfiguration<Logs>
        {
            public void Configure(EntityTypeBuilder<Logs> builder)
            {
                builder.Property(r => r.UserName).HasMaxLength(255);
                builder.Property(r => r.ResourceId).HasMaxLength(255);
                builder.Property(r => r.LogType).HasMaxLength(255).IsRequired();
            }
        }
    }
}