// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Shashlik.RC.Client
{
    /// <summary>
    /// 响应结果
    /// </summary>
    public class ResponseResult<T>
    {
        /// <summary>
        /// 错误代码,1:正常,其他的为业务错误代码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? Msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 调试信息
        /// </summary>
        public string? Debug { get; set; }
    }
}