using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace Shashlik.RC.Server.Data.PostgreSql
{
    public static class Extensions
    {
        public static void AddNpgsqlData(this IServiceCollection services, string connString)
        {
            services.AddDbContext<RCDbContext>(r =>
            {
                r.UseNpgsql(connString,
                    builder => builder.MigrationsAssembly(typeof(Extensions).Assembly.GetName().FullName));
            });
        }
    }
}