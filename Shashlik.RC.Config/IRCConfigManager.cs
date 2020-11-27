namespace Shashlik.RC.Config
{
    public interface IRCConfigManager
    {
        /// <summary>
        /// 获取指定配置的内容
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <returns></returns>
        ConfigModel Get(string configName);
    }
}
