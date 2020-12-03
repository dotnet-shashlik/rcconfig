using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Shashlik.RC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(r =>
                {
                    var file = new FileInfo("./data/appsettings.yaml").FullName;
                    r.AddYamlFile(file);
                })
                .ConfigureWebHostDefaults((webBuilder) =>
                {
#if DEBUG
                    // 正式服使用环境变量 ASPNETCORE_URLS=http://*:5000 来灵活配置
                    webBuilder.UseUrls("http://*:5000");
#endif
                    webBuilder.UseStartup<Startup>();
                });
    }
}