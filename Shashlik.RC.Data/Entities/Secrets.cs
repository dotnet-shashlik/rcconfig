using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    public class Secrets : IEntity<int>
    {
        public int Id { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public long CreateTime { get; set; }

        public int EnvironmentId { get; set; }

        public Environments Environment { get; set; }


        public class Configs : IEntityTypeConfiguration<Secrets>
        {
            public void Configure(EntityTypeBuilder<Secrets> builder)
            {
                builder.Property(r => r.SecretId).HasMaxLength(255).IsRequired();
                builder.Property(r => r.SecretKey).HasMaxLength(255).IsRequired();
                builder.HasIndex(r => r.SecretId).IsUnique();

                builder.HasOne(r => r.Environment)
                    .WithMany(r => r.Secrets)
                    .HasForeignKey(r => r.EnvironmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}