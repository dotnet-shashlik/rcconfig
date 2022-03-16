using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Server.Data.Sqlite
{
    public static class Extensions
    {
        public static void AddSqliteData(this IServiceCollection services, string connString)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseSqlite(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
#if DEBUG
                r.LogTo(Console.WriteLine);
#endif
            });
        }
    }
}