using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// 配置内容
    /// </summary>
    public class Configs : IEntity<int>, ISoftDeleted<DateTime>
    {
        /// <summary>
        /// 配置id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 环境id
        /// </summary>
        public int EnvId { get; set; }

        /// <summary>
        /// 环境
        /// </summary>
        public Envs Env { get; set; }

        /// <summary>
        /// 配置内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 配置类型,json,xml,yaml,ini....
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 配置名称是否作为配置节点,1:true,0:false
        ///// </summary>
        //public int NameIsSection { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 配置是否启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 修改记录
        /// </summary>
        public List<ModifyRecords> ModifyRecords { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }
    }

    public class ConfigsConfig : IEntityTypeConfiguration<Configs>
    {
        public void Configure(EntityTypeBuilder<Configs> builder)
        {
            builder.HasOne(r => r.Env).WithMany(r => r.Configs).HasForeignKey(r => r.EnvId);

            builder.Property(r => r.Type).HasMaxLength(32).IsRequired();
            builder.Property(r => r.Name).HasMaxLength(32).IsRequired();
            builder.HasIndex(r => new {r.EnvId, r.Name}).IsUnique();

            builder.Property(r => r.Desc).HasMaxLength(512);
        }
    }
}