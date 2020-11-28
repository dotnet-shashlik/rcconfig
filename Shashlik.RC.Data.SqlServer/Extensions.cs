using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.EfCore;

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

            if (autoMigration)
                services.Migration<RCDbContext>();
        }
    }
}