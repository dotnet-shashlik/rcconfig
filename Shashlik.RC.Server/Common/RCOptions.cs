using FreeSql;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Server.Common;

[AutoOptions("RC")]
public class RCOptions
{
    /// <summary>
    /// token有效期,秒,默认半小时
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 1800;

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DataType DbType { get; set; } = DataType.Sqlite;

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string DbConn { get; set; } = "Data Source=./data/rc.db;";

    /// <summary>
    /// 管理员用户名
    /// </summary>
    public string AdminUser { get; set; } = "admin";

    /// <summary>
    /// 管理员密码
    /// </summary>
    public string AdminPassword { get; set; } = "Shashlik.RC.Server";

    /// <summary>
    /// 集群服务器,不支持自动扩容
    /// </summary>
    public string? Servers { get; set; }

    /// <summary>
    /// 是否为单机版
    /// </summary>
    public bool IsStandalone => string.IsNullOrWhiteSpace(Servers);

    /// <summary>
    /// 集群环境,服务器内部安全认证token,集群环境请一定要设置
    /// </summary>
    public string? ServerToken { get; set; }

    /// <summary>
    /// token 签名密钥,24位
    /// </summary>
    public string SignKey { get; set; } = RandomHelper.RandomString(24);
}