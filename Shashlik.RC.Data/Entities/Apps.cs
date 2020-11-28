using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore;

namespace Shashlik.RC.Data.Entities
{
    /// <summary>
    /// 应用
    /// </summary>
    public class Apps : IEntity<string>
    {
        /// <summary>
        /// appid
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 登录密码(MD5大写)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// app名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// app描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 下属环境变量
        /// </summary>
        public List<Envs> Envs { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否已启用
        /// </summary>
        public bool Enabled { get; set; }
    }

    public class AppsConfig : IEntityTypeConfiguration<Apps>
    {
        public void Configure(EntityTypeBuilder<Apps> builder)
        {
            builder.Property(r => r.Id).HasMaxLength(32);

            builder.Property(r => r.Name).HasMaxLength(32).IsRequired();
            builder.Property(r => r.Desc).HasMaxLength(512);
        }
    }
}
