using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.SqlLite
{
    public static class Extensions
    {
        public static void AddRCDataSqlLite(this IServiceCollection services, string connString,
            bool autoMigration = false)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseSqlite(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });

            if (autoMigration)
                using (var serviceProvider = services.BuildServiceProvider())
                using (var scope = serviceProvider.CreateScope())
                using (var dbContext = scope.ServiceProvider.GetService<RCDbContext>())
                    dbContext.Database.Migrate();
        }
    }
}