using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.Sqlite
{
    public static class Extensions
    {
        public static void AddSqliteData(this IServiceCollection services, string connString)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseSqlite(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}