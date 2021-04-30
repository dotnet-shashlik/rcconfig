using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    public class SignatureKeys : IEntity<int>
    {
        public int Id { get; set; }

        public string PrivateKey { get; set; }

        public string PublicKey { get; set; }

        public string KeyType { get; set; }

        public bool Enabled { get; set; }

        public class Configs : IEntityTypeConfiguration<SignatureKeys>
        {
            public void Configure(EntityTypeBuilder<SignatureKeys> builder)
            {
                builder.Property(r => r.PrivateKey).HasMaxLength(2048).IsRequired();
                builder.Property(r => r.PublicKey).HasMaxLength(2048);
                builder.Property(r => r.KeyType).HasMaxLength(255).IsRequired();
                // 每种密钥只能有条数据
                builder.HasIndex(r => r.KeyType).IsUnique();
            }
        }
    }
}