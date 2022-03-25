using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Shashlik.Kernel;
using Shashlik.RC.Server.Initialization;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.FreeSql;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Shashlik.RC.Server;

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
        var rcOptions = Configuration.GetSection("RC")
            .Get<RCOptions>();
        var conn = rcOptions.DbConn;
        var dbType = rcOptions.DbType;

        var freeSql = new FreeSqlBuilder()
            .UseConnectionString(dbType, conn)
            .CreateDatabaseIfNotExists()
            .UseAutoSyncStructure(true)
            .BuildAsyncTransactionSupport();
        services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddFreeSqlStoresWithIntKey(freeSql);
        services.AddSingleton(freeSql);

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


        services.AddAuthorization();
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
            .InitRoleAndAdminUser()
            .AssembleServiceProvider()
            ;

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        //app.UseSpa(spa => { spa.Options.SourcePath = "AdminUI"; });
    }
}