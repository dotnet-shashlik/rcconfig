using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            var rcOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<RCOptions>>();

            if (rcOptions.Value.IsStandalone)
            {
                context.HttpContext.Response.StatusCode = 400;
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(Constants.HeaderKeys.ServerToken, out var token)
                || token.ToString() != rcOptions.Value.ServerToken)
            {
                context.HttpContext.Response.StatusCode = 400;
            }
        }
    }
}