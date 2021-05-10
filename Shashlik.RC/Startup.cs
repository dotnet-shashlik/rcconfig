using System;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shashlik.AspNetCore;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.RC.Common;
using Shashlik.RC.Data;
using Shashlik.RC.Data.MySql;
using Shashlik.RC.Data.PostgreSql;
using Shashlik.RC.Data.Sqlite;
using Shashlik.RC.Data.SqlServer;
using Shashlik.RC.IdentityServer;
using Shashlik.RC.WebSocket;
using Shashlik.Utils.Extensions;

// ReSharper disable ConditionIsAlwaysTrueOrFalse


namespace Shashlik.RC
{
    /**
     * dotnet ef migrations add rc_0001 -c RCDbContext -o Migrations -p ./Shashlik.RC.Data.Sqlite/Shashlik.RC.Data.Sqlite.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
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
            var conn = Environment.GetEnvironmentVariable("DB_CONN");
            if (conn.IsNullOrWhiteSpace())
                conn = "Data Source=./data/rc.db;";
            var dbType = Environment.GetEnvironmentVariable("DB_TYPE");
            if (dbType.IsNullOrWhiteSpace())
                dbType = "sqlite";

            switch (dbType)
            {
                case "sqlite":
                    services.AddSqliteData(conn);
                    break;
                case "mysql":
                    services.AddMySqlData(conn);
                    break;
                case "npgsql":
                    services.AddNpgsqlData(conn);
                    break;
                case "sqlserver":
                    services.AddSqlServerData(conn);
                    break;
                default: throw new InvalidOperationException("invalid db type");
            }

            // 增加认证服务
            services.AddAuthentication(r =>
                {
                    r.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    r.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    r.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    r.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    r.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                // 增加jwt认证
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, r =>
                {
                    r.Audience = Ids4Extensions.Api;
                    r.Authority = SystemEnvironmentUtils.Authority;
                });

            services.AddAuthorization();
            services.AddControllers();
            services.AddMediatR(GetType().Assembly);
            services.AddHttpContextAccessor();
            services.AddIdentity<IdentityUser<int>, IdentityRole<int>>()
                .AddEntityFrameworkStores<RCDbContext>();
            services.AddShashlik(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.ApplicationServices.UseShashlik()
                .DoAutoMigration()
                .AutowireServiceProvider()
                .AutowireAspNet(app);

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
        }
    }
}