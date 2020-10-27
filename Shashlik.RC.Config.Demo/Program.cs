using Jinkong.RC.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Shashlik.RC.Config.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseRCConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseUrls("http://*:6001")
                        .UseStartup<Startup>()
                        ;
                });
    }
}