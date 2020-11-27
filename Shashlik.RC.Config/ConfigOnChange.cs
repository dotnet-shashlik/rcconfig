using System;

namespace Shashlik.RC.Config
{
    /// <summary>
    /// 配置变化事件
    /// </summary>
    class ConfigOnChange
    {
        /// <summary>
        /// string(configName)
        /// </summary>
        public static Action<string> OnChange;
    }
}
