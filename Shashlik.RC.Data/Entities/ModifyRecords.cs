using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// 修改记录
    /// </summary>
    public class ModifyRecords : IEntity<int>
    {
        public int Id { get; set; }

        public int ConfigId { get; set; }

        public Configs Config { get; set; }

        public DateTime ModifyTime { get; set; }

        public string Desc { get; set; }

        public string BeforeContent { get; set; }

        public string AfterContent { get; set; }
    }

    public class ModifyRecordsConfig : IEntityTypeConfiguration<ModifyRecords>
    {
        public void Configure(EntityTypeBuilder<ModifyRecords> builder)
        {
            builder.HasOne(r => r.Config)
                .WithMany(r => r.ModifyRecords)
                .HasForeignKey(r => r.ConfigId)
                .IsRequired(false);
        }
    }
}