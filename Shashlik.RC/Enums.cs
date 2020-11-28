// ReSharper disable InconsistentNaming

namespace Shashlik.RC
{
    public static class Enums
    {
        public enum ConfigType
        {
            /// <summary>
            /// json配置
            /// </summary>
            json = 1,

            // 不再支持xml
            // /// <summary>
            // /// xml配置
            // /// </summary>
            // xml = 2,
            /// <summary>
            /// yaml配置
            /// </summary>
            yaml = 3
        }
    }
}