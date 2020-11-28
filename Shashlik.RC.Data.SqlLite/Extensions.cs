﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.EfCore;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.SqlLite
{
    public static class Extensions
    {
        public static void AddSqlLiteData(this IServiceCollection services, string connString)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseSqlite(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}