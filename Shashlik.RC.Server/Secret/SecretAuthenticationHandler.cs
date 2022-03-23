using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Services.Identity;
using Shashlik.RC.Server.Services.Secret;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Server.Secret
{
    public class SecretAuthenticationHandler : IAuthenticationHandler
    {
        public const string SecretScheme = "Secret";

        /// <summary>
        /// 认证体系
        /// </summary>
        public AuthenticationScheme? Scheme { get; private set; }

        /// <summary>
        /// 当前上下文
        /// </summary>
        protected HttpContext? Context { get; private set; }

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
            if (!Context!.Request.RouteValues.TryGetValue(Constants.ResourceRoute.ApplicationKey, out _))
                return AuthenticateResult.Fail("invalid request path");

            var secretId = Context.Request.Headers.GetValue("SecretId");
            var timestamp = Context.Request.Headers.GetValue("Timestamp");
            var sign = Context.Request.Headers.GetValue("Sign");
            var nonce = Context.Request.Headers.GetValue("Nonce");
            if (secretId.IsNullOrWhiteSpace())
                return AuthenticateResult.Fail("require SecretId");
            if (timestamp.IsNullOrWhiteSpace())
                return AuthenticateResult.Fail("require Timestamp");
            if (sign.IsNullOrWhiteSpace())
                return AuthenticateResult.Fail("require Sign");
            if (nonce.IsNullOrWhiteSpace())
                return AuthenticateResult.Fail("require Nonce");


            var secretService = Context.Request.HttpContext.RequestServices.GetRequiredService<SecretService>();
            var secretDto = await secretService.GetBySecretId(secretId!);
            if (secretDto is null)
                return AuthenticateResult.Fail("invalid SecretId");

            var signData = new Dictionary<string, string?>
            {
                { "SecretId", secretId },
                { "Timestamp", timestamp },
                { "Nonce", nonce }
            };

            foreach (var keyValuePair in Context.Request.Query)
                signData[keyValuePair.Key] = keyValuePair.Value;
            var body = await Context.Request.Body.ReadToStringAsync();
            signData["Body"] = body;

            var signStr = signData.OrderBy(r => r.Key)
                .Select(r => $"{r.Key}={r.Value}")
                .Join("&");
            var signValue = HashHelper.HMACSHA256(signStr, secretDto.SecretKey);
            if (signValue != sign)
                return AuthenticateResult.Fail("invalid Sign");

            var userService = Context.Request.HttpContext.RequestServices.GetRequiredService<UserService>();
            var user = await userService.Get(secretDto.UserId.ParseTo<int>());

            //TODO: user claims
            var claimsIdentity = new ClaimsIdentity(new[]
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
            Context!.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            Context!.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }
    }
}