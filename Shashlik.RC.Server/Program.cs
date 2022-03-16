﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Shashlik.RC.Server
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
                .ConfigureAppConfiguration(r => { r.AddYamlFile("./data/appsettings.yaml", true); })
                .ConfigureWebHostDefaults((webBuilder) => { webBuilder.UseStartup<Startup>(); });
    }
}