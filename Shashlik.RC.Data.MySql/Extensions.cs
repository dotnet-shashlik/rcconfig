using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}