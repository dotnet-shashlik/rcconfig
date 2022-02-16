using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Shashlik.RC.Common;
using Shashlik.RC.Services.Identity;
using Shashlik.RC.Services.Secret;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Secret
{
    public class SecretAuthenticationHandler : IAuthenticationHandler
    {
        public const string SecretScheme = "Secret";

        /// <summary>
        /// 认证体系
        /// </summary>
        public AuthenticationScheme Scheme { get; private set; }

        /// <summary>
        /// 当前上下文
        /// </summary>
        protected HttpContext Context { get; private set; }

        /// <summary>
        /// 初始化认证
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            Scheme = scheme;
            Context = context;
            return Task.CompletedTask;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            // secret认证仅允许访问资源api
            if (!Context.Request.RouteValues.TryGetValue(Constants.ResourceRoute.ApplicationKey, out _))
            {
                return AuthenticateResult.Fail("invalid request path");
            }

            if (!Context.Request.Headers.TryGetValue("SecretId", out var secretIdValues))
            {
                return AuthenticateResult.Fail("require SecretId");
            }

            if (!Context.Request.Headers.TryGetValue("Timestamp", out var timestampValues))
            {
                return AuthenticateResult.Fail("require Timestamp");
            }

            if (!Context.Request.Headers.TryGetValue("Sign", out var signValues))
            {
                return AuthenticateResult.Fail("require Sign");
            }

            var secretService = Context.Request.HttpContext.RequestServices.GetRequiredService<SecretService>();
            var secretDto = await secretService.GetBySecretId(secretIdValues.ToString());
            if (secretDto is null)
                return AuthenticateResult.Fail("invalid SecretId");

            if (Context.Request.ContentType == "application/json")
            {
                var body = await Context.Request.Body.ReadToStringAsync();
                var str = timestampValues + secretDto.SecretKey + body;
                var calcSign = HashHelper.MD5(str);
                if (signValues.ToString() != calcSign)
                    return AuthenticateResult.Fail("invalid Sign");
            }
            else
            {
                var str = Context.Request.Query
                    .Concat(new Dictionary<string, StringValues>()
                    {
                        { "Timestamp", timestampValues },
                        { "SecretKey", secretDto.SecretKey },
                    })
                    .OrderBy(r => r.Key)
                    .Select(r => $"{r.Key}={r.Value}")
                    .Join("&");
                var calcSign = HashHelper.MD5(str);
                if (signValues.ToString() != calcSign)
                    return AuthenticateResult.Fail("invalid Sign");
            }

            var userService = Context.Request.HttpContext.RequestServices.GetRequiredService<UserService>();
            var user = await userService.Get(secretDto.UserId.ParseTo<int>());

            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", user!.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            }, SecretScheme);
            user.Roles.ForEachItem(r => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, r)));

            var principal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(principal, SecretScheme);
            return AuthenticateResult.Success(ticket);
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            Context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }
    }
}