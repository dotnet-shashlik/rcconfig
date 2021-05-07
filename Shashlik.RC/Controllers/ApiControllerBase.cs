using System;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.AspNetCore.Filters;
using Shashlik.RC.Services;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ExceptionWrapper]
    [ResponseWrapper]
    public class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase()
        {
            _lazyUser = new Lazy<IdentityUser<int>?>(() => User.IsAuthenticated()
                ? HttpContext.RequestServices.GetRequiredService<UserServices>().FindByIdAsync(User
                    .FindFirstValue(JwtClaimTypes.Subject)).GetAwaiter().GetResult()
                : null);
            _lazyLoginUserId = new Lazy<int>(() => User.FindFirstValue(JwtClaimTypes.Subject).ParseTo<int>());
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
    }
}