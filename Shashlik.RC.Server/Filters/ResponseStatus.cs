namespace Shashlik.RC.Server.Filters
{
    public enum ResponseStatus
    {
        /// <summary>
        /// 其它
        /// </summary>
        Other = 499,

        /// <summary>
        /// 参数错误
        /// </summary>
        ArgError = 400,

        /// <summary>
        /// 操作/逻辑错误
        /// </summary>
        LogicalError = 498,

        /// <summary>
        /// 未认证
        /// </summary>
        UnAuthentication = 401,

        /// <summary>
        /// 拒绝请求
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 资源不存在
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 系统错误
        /// </summary>
        SystemError = 500
    }
}