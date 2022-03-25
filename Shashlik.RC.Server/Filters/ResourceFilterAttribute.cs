using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Services.Resource;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Filters;

/// <summary>
/// 资源过滤器
/// </summary>
public class ResourceFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string resourceId = string.Empty;

        if (context.RouteData.Values.TryGetValue(Constants.ResourceRoute.ApplicationKey, out var application))
        {
            if (application is null)
            {
                context.HttpContext.Response.StatusCode = 400;
                return;
            }

            resourceId += application.ToString();
        }

        if (context.RouteData.Values.TryGetValue(Constants.ResourceRoute.EnvironmentKey, out var environment))
        {
            if (environment is null)
            {
                context.HttpContext.Response.StatusCode = 400;
                return;
            }

            resourceId += "/" + environment;
        }

        if (!string.IsNullOrWhiteSpace(resourceId) && !(context.HttpContext.User.Identity?.IsAuthenticated ?? false))
        {
            context.HttpContext.Response.StatusCode = 403;
            return;
        }

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier).ParseTo<int>();

        if (!resourceId.IsNullOrWhiteSpace() && !context.HttpContext.User.IsInRole(Constants.Roles.Admin))
        {
            var permissionService = context.HttpContext.RequestServices.GetRequiredService<ResourceService>();
            var hasPermission = permissionService
                .HasPermission(userId, resourceId, HttpMethod2PermissionAction(context.HttpContext.Request.Method))
                .GetAwaiter().GetResult();

            if (!hasPermission)
                context.HttpContext.Response.StatusCode = 403;
        }
    }

    private PermissionAction HttpMethod2PermissionAction(string method)
    {
        if (method.EndsWithIgnoreCase("get"))
            return PermissionAction.Read;
        if (method.EndsWithIgnoreCase("post")
            || method.EndsWithIgnoreCase("put")
            || method.EndsWithIgnoreCase("patch")
           )
            return PermissionAction.Write;
        if (method.EndsWithIgnoreCase("delete"))
            return PermissionAction.Delete;

        throw new InvalidCastException($"invalid http method \"{method}\" on resource check");
    }
}