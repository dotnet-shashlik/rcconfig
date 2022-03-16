using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Server.Data.Entities
{
    public class Secrets
    {
        public int Id { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public long CreateTime { get; set; }

        public string UserId { get; set; }

        public class Configs : IEntityTypeConfiguration<Secrets>
        {
            public void Configure(EntityTypeBuilder<Secrets> builder)
            {
                builder.Property(r => r.SecretId).HasMaxLength(255).IsRequired();
                builder.Property(r => r.SecretKey).HasMaxLength(255).IsRequired();
                builder.HasIndex(r => r.SecretId).IsUnique();
                builder.Property(r => r.UserId).HasMaxLength(32).IsRequired();
            }
        }
    }
}