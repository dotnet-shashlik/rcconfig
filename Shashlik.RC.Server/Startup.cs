using System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Shashlik.Kernel;
using Shashlik.RC.Data;
using Shashlik.RC.Data.MySql;
using Shashlik.RC.Data.PostgreSql;
using Shashlik.RC.Data.Sqlite;
using Shashlik.RC.Data.SqlServer;
using Shashlik.RC.Server.Initialization;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Data;
using Shashlik.RC.Server.WebSocket;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Shashlik.RC.Server
{
    /**
     * Sqlite: dotnet ef migrations add rc_0001 -c RCDbContext -o Migrations -p ./Shashlik.RC.Data.Sqlite/Shashlik.RC.Data.Sqlite.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
     * MySql: dotnet ef migrations add rc_0001 -c RCDbContext -o Migrations -p ./Shashlik.RC.Data.MySql/Shashlik.RC.Data.MySql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
     * PostgreSql: dotnet ef migrations add rc_0001 -c RCDbContext -o Migrations -p ./Shashlik.RC.Data.PostgreSql/Shashlik.RC.Data.PostgreSql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
     * SqlServer: dotnet ef migrations add rc_0001 -c RCDbContext -o Migrations -p ./Shashlik.RC.Data.SqlServer/Shashlik.RC.Data.SqlServer.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
     */
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var conn = SystemEnvironmentUtils.DbConn;
            var dbType = SystemEnvironmentUtils.DbType;

            switch (dbType)
            {
                case Constants.Db.Sqlite:
                    services.AddSqliteData(conn);
                    break;
                case Constants.Db.MySql:
                    services.AddMySqlData(conn);
                    break;
                case Constants.Db.PostgreSql:
                    services.AddNpgsqlData(conn);
                    break;
                case Constants.Db.SqlServer:
                    services.AddSqlServerData(conn);
                    break;
                default: throw new InvalidOperationException("invalid db type");
            }

            services.AddControllers()
                .AddControllersAsServices();
            services.AddCors(r =>
            {
                r.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod();
                });
            });
            services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<RCDbContext>()
                ;

            services.AddAuthorization();
            services.AddMediatR(GetType().Assembly);
            // services.AddSpaStaticFiles(r => { r.RootPath = "AdminUI/dist"; });
            services.AddShashlik(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            IdentityModelEventSource.ShowPII = true;

            app.UseCors();
            app.UseStaticFiles();
            //app.UseSpaStaticFiles();
            app.UseRouting();

            app.ApplicationServices.UseShashlik()
                .MigrationDb()
                .InitRoleAndAdminUser()
                .AssembleServiceProvider()
                ;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSocketPush();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseSpa(spa => { spa.Options.SourcePath = "AdminUI"; });
        }
    }
}