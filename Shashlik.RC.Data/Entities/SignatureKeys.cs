using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.RC.Data.Entities
{
    public class SignatureKeys
    {
        public int Id { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        public string KeyType { get; set; }

        public bool Enabled { get; set; }

        public long CreateTime { get; set; }

        public class Configs : IEntityTypeConfiguration<SignatureKeys>
        {
            public void Configure(EntityTypeBuilder<SignatureKeys> builder)
            {
                builder.Property(r => r.PrivateKey).HasMaxLength(2048).IsRequired();
                builder.Property(r => r.PublicKey).HasMaxLength(2048);
                builder.Property(r => r.KeyType).HasMaxLength(255).IsRequired();
            }
        }
    }
}