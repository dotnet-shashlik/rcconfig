using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class Logs : IEntity<long>
    {
        public long Id { get; set; }

        public long LogTime { get; set; }

        public int LogUserId { get; set; }

        public string LogUserName { get; set; }

        /// <summary>
        /// 日志概要信息
        /// </summary>
        public string Summary { get; set; }

        public int? FileId { get; set; }
        public string FileName { get; set; }
        public int? ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int? EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }

        public string BeforeContent { get; set; }

        public string AfterContent { get; set; }

        public class Configs : IEntityTypeConfiguration<Logs>
        {
            public void Configure(EntityTypeBuilder<Logs> builder)
            {
                builder.Property(r => r.LogUserName).HasMaxLength(255);
                builder.Property(r => r.Summary).HasMaxLength(255);
                builder.Property(r => r.FileName).HasMaxLength(255);
                builder.Property(r => r.ApplicationName).HasMaxLength(255);
                builder.Property(r => r.EnvironmentName).HasMaxLength(255);
            }
        }
    }
}