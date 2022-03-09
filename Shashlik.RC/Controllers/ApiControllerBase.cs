using System;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.RC.Filters;
using Shashlik.RC.Services.Identity;
using Shashlik.Utils.Extensions;
using Shashlik.RC.Common;

namespace Shashlik.RC.Controllers
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
            _lazyUser = new Lazy<IdentityUser<int>?>(() => User.IsAuthenticated()
                ? HttpContext.RequestServices.GetRequiredService<UserService>().FindByIdAsync(User.Identity.GetSubjectId()).GetAwaiter().GetResult()
                : null);
            _lazyLoginUserId = new Lazy<int>(() => User.Identity.GetSubjectId().ParseTo<int>());
            _logger = new Lazy<ILogger>(() => HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType()));
        }

        private readonly Lazy<int> _lazyLoginUserId;
        private readonly Lazy<IdentityUser<int>?> _lazyUser;
        private readonly Lazy<ILogger> _logger;

        /// <summary>
        /// 当前登录用户id,未登录:null
        /// </summary>
        protected int? LoginUserId => User.IsAuthenticated() ? _lazyLoginUserId.Value : null;

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