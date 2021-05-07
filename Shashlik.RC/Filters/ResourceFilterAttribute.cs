using System;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Permission;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Filters
{
    /// <summary>
    /// 资源过滤器
    /// </summary>
    public class ResourceFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string resourceId = string.Empty;

            if (context.RouteData.Values.TryGetValue(Constants.ResourceRoute.ApplicationId, out var applicationId))
            {
                if (applicationId is null)
                {
                    context.HttpContext.Response.StatusCode = 400;
                    return;
                }

                resourceId += applicationId.ToString();
            }

            if (context.RouteData.Values.TryGetValue(Constants.ResourceRoute.EnvironmentId, out var environmentId))
            {
                if (environmentId is null)
                {
                    context.HttpContext.Response.StatusCode = 400;
                    return;
                }

                resourceId += "/" + environmentId;
            }

            if (!context.HttpContext.User.IsAuthenticated())
            {
                context.HttpContext.Response.StatusCode = 403;
                return;
            }

            var userId = context.HttpContext.User.FindFirstValue(JwtClaimTypes.Subject).ParseTo<int>();

            if (!resourceId.IsNullOrWhiteSpace() && !context.HttpContext.User.IsInRole(Constants.Roles.Admin))
            {
                var permissionService = context.HttpContext.RequestServices.GetRequiredService<PermissionService>();
                bool hasPermission;
                if (SystemEnvironmentUtils.PermissionReadPolicy == PermissionReadPolicy.Db)
                {
                    hasPermission = permissionService
                        .HasPermission(userId, resourceId, HttpMethod2PermissionAction(context.HttpContext.Request.Method))
                        .GetAwaiter().GetResult();
                }
                else if (SystemEnvironmentUtils.PermissionReadPolicy == PermissionReadPolicy.Token)
                {
                    hasPermission = permissionService.HasPermission(userId, resourceId,
                        HttpMethod2PermissionAction(context.HttpContext.Request.Method), context.HttpContext.User.Claims);
                }
                else
                    throw new IndexOutOfRangeException();

                if (!hasPermission)
                {
                    context.HttpContext.Response.StatusCode = 403;
                }
            }
        }

        private PermissionAction HttpMethod2PermissionAction(string method)
        {
            if (method.EndsWithIgnoreCase("get"))
                return PermissionAction.Read;
            if (method.EndsWithIgnoreCase("post")
                || method.EndsWithIgnoreCase("put"))
                return PermissionAction.Write;
            if (method.EndsWithIgnoreCase("delete"))
                return PermissionAction.Delete;

            throw new InvalidCastException($"invalid http method \"{method}\" on resource check");
        }
    }
}