using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.EfCore;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Data.MySql
{
    public static class Extensions
    {
        public static void AddMySqlData(this IServiceCollection services, string connString, string version)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseMySql(connString, ServerVersion.FromString(version),
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}