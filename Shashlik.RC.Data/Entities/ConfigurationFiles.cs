﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    public class ConfigurationFiles : IEntity<string>
    {
        /// <summary>
        /// 文件id
        /// </summary>
        public string Id { get; set; }

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
        /// 环境id
        /// </summary>
        public int EnvironmentId { get; set; }

        /// <summary>
        /// 所属环境
        /// </summary>
        public Environments Environment { get; set; }

        public class Configs : IEntityTypeConfiguration<ConfigurationFiles>
        {
            public void Configure(EntityTypeBuilder<ConfigurationFiles> builder)
            {
                builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
                builder.Property(r => r.Type).HasMaxLength(255).IsRequired();
                builder.Property(r => r.Desc).HasMaxLength(255);

                builder.HasOne(r => r.Environment)
                    .WithMany(r => r.Files)
                    .HasForeignKey(r => r.EnvironmentId);
            }
        }
    }
}