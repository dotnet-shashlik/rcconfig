﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Data;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.SqlServer
{
    public static class Extensions
    {
        public static void AddSqlServerData(this IServiceCollection services, string connString,
            bool autoMigration = false)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseSqlServer(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}