using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Server.Data.SqlServer
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