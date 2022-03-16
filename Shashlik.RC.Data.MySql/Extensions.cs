﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Data;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.MySql
{
    public static class Extensions
    {
        public static void AddMySqlData(this IServiceCollection services, string connString)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseMySql(connString, ServerVersion.AutoDetect(connString),
                    builder => builder.MigrationsAssembly("Shashlik.RC.Data.MySql"));
            });
        }
    }
}