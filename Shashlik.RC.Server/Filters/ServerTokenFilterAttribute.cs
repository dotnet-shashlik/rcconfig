using Microsoft.AspNetCore.Mvc.Filters;
using Shashlik.RC.Server.Common;

namespace Shashlik.RC.Server.Filters
{
    /// <summary>
    /// 内部服务器安全认证
    /// </summary>
    public class ServerTokenFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (SystemEnvironmentUtils.IsStandalone)
            {
                context.HttpContext.Response.StatusCode = 400;
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(Constants.HeaderKeys.ServerToken, out var token)
                || token.ToString() != SystemEnvironmentUtils.ServerToken)
            {
                context.HttpContext.Response.StatusCode = 400;
            }
        }
    }
}