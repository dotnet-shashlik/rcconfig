# Shashlik RC 远程配置中心

---
Shashlik RC是一个使用.net core 开发的远程配置中心。功能非常简单实用，就是存储程序的配置文件，目前支持json和yaml格式，以解决本地文件式配置的繁琐和管理问题，比如集群、分布式、容器环境下文件式配置会让人崩溃。那么Shashlik RC就是为了解决这些问题而出现的。它没有那么多繁琐、庞大的功能，它仅仅只是将的文件配置统一在配置中心管理，如果你需要服务注册？权限管理？流程治理？那么Shashlik RC显然不是适合你。Shashlik RC目前只支持单机部署，使用websocket实时推送配置更新，占用资源少非常少。


##  docker快速启动server

```
sudo git clone https://github.com/dotnet-shashlik/rcconfig.git

sudo docker-compose up -d
```

默认配置
```yaml
# 绑定url、端口
ASPNETCORE_URLS: http://*:8989
# 管理员账户
ADMIN_USER: admin
# 管理密码
ADMIN_PASS: 123123
# 数据库类型: sqlite/mysql/npgsql/sqlserver
DB_TYPE: sqlite
# 数据库连接字符串
CONN: Data Source=./data/rc.db;
```

嗯，就这么简单，你就启动了一个Shashlik RC 服务端。可以修改docker-compose.yml文件的环境变量以使用实际的配置。访问地址：http://<your host>/Account/Login。

## 新增应用

使用管理账户、密码登录管理中心，新增应用。AppId才是具体应用登录的账户。


## 登录应用

使用AppId和新增应用时设置的密码进行登录。

## 新增配置环境

例Devolepment/Test/Production，客户端将直接使用当前程序的环境名称。

## 新增配置文件

就像新增本地配置文件一样

## .net core客户端连接

1. 安装nuget包
```
Install-Package Shashlik.RC.Config

```

2. 配置连接
在appsettings.json中增加server
```json
{
  "RCConfig": {
    "Server": "<your server host>",
    "AppId": "<app id>",
    "AppKey": "<app key>",
    // 轮询获取配置的间隔，单位秒，0：不配置
    "Polling": 300
  }
}
```

3. 启用远程配置

`.UseRCConfiguration()`即启用远程配置。

```c#
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                // 启动远程配置
                .UseRCConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
```

4. 配置websocket实时推送
```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
        app.UseRouting();
        app.UseRCRealTimeUpdate();
        // ...
    }
```