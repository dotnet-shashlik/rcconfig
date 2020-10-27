using System;
using System.Collections.Generic;
using System.Text;

namespace Jinkong.RC.Config
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
