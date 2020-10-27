namespace Jinkong.RC.Config
{
    class RCConfigManager : IRCConfigManager
    {
        public ConfigModel Get(string configName)
        {
            var result = RequestHelper.Get(configName);

            return new ConfigModel
            {
                Content = result["content"]?.ToString(),
                Name = result["name"]?.ToString(),
                Type = result["type"]?.ToString()
            };
        }
    }
}
