using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.Utils.Extensions;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Filters;
using Shashlik.RC.Server.Services.Identity;

namespace Shashlik.RC.Server.Controllers
{
    [Authorize(AuthenticationSchemes = "Secret,JwtBearer")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ExceptionWrapper]
    [ResponseWrapper]
    [ResourceFilter]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase()
        {
            _lazyUser = new Lazy<IdentityUser<int>?>(() => User.Identity?.IsAuthenticated ?? false
                ? HttpContext.RequestServices.GetRequiredService<UserService>().FindByIdAsync(User.GetUserId().ToString()).GetAwaiter()
                    .GetResult()
                : null);
            _lazyLoginUserId = new Lazy<int?>(() => User.GetUserId()?.ParseTo<int>());
            _logger = new Lazy<ILogger>(() => HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType()));
        }

        private readonly Lazy<int?> _lazyLoginUserId;
        private readonly Lazy<IdentityUser<int>?> _lazyUser;
        private readonly Lazy<ILogger> _logger;

        /// <summary>
        /// 当前登录用户id,未登录:null
        /// </summary>
        protected int? LoginUserId => _lazyLoginUserId.Value;

        /// <summary>
        /// 当前登录用户信息,未登录:null,会执行数据库查询,当只需要用户id时请使用<see cref="LoginUserId"/>
        /// </summary>
        protected IdentityUser<int>? LoginUserInfo => _lazyUser.Value;

        /// <summary>
        /// 日志
        /// </summary>
        protected ILogger Logger => _logger.Value;

        protected string GetResourceId()
        {
            string resourceId = string.Empty;

            if (HttpContext.Request.RouteValues.TryGetValue(Constants.ResourceRoute.ApplicationKey, out var application))
                resourceId += application?.ToString();

            if (string.IsNullOrWhiteSpace(resourceId))
                return resourceId;

            if (HttpContext.Request.RouteValues.TryGetValue(Constants.ResourceRoute.EnvironmentKey, out var environment))
                resourceId += "/" + environment;

            return resourceId;
        }
    }
}