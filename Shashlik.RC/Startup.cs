using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel;
using Shashlik.RC.Data.MySql;
using Shashlik.RC.Data.PostgreSql;
using Shashlik.RC.Data.Sqlite;
using Shashlik.RC.Data.SqlServer;
using Shashlik.RC.WebSocket;
using Shashlik.Utils.Extensions;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Shashlik.RC
{
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
            var conn = Configuration.GetValue<string>("DB_CONN");
            if (conn.IsNullOrWhiteSpace())
                conn = Environment.GetEnvironmentVariable("DB_CONN");
            if (conn.IsNullOrWhiteSpace())
                conn = "Data Source=./data/rc.db;";
            var dbType = Configuration.GetValue<string>("DB_TYPE");
            if (dbType.IsNullOrWhiteSpace())
                dbType = Environment.GetEnvironmentVariable("DB_TYPE");
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddAuthentication(r => { r.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                .AddCookie(options =>
                {
                    options.LoginPath = "/account/login";
                    options.AccessDeniedPath = "/account/login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            services.AddAuthorization();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddAntiforgery();

            services.AddSingleton<WebSocketContext>();

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

            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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

// sqlite:         dotnet ef migrations add AddAccountLock -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.SqlLite/Shashlik.RC.Data.SqlLite.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
// mysql:          dotnet ef migrations add AddAccountLock -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.MySql/Shashlik.RC.Data.MySql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
// postgresql:     dotnet ef migrations add AddAccountLock -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.PostgreSql/Shashlik.RC.Data.PostgreSql.csproj -s ./Shashlik.RC/Shashlik.RC.csproj
// sqlserver:      dotnet ef migrations add AddAccountLock -c RCDbContext  -o Migrations -p ./Shashlik.RC.Data.SqlServer/Shashlik.RC.Data.SqlServer.csproj -s ./Shashlik.RC/Shashlik.RC.csproj