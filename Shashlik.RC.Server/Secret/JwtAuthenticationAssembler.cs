using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Secret;

[Transient]
public class JwtAuthenticationAssembler : IServiceAssembler
{
    public const string Iss = "shashlik.rc.iss";
    public const string Aud = "shashlik.rc.aud";


    public JwtAuthenticationAssembler(IOptions<RCOptions> rcOptions)
    {
        RcOptions = rcOptions;
    }

    private IOptions<RCOptions> RcOptions { get; }


    public void Configure(IKernelServices kernelServices)
    {
        if (RcOptions.Value.SignKey.IsNullOrWhiteSpace() || RcOptions.Value.SignKey!.Length != 24)
            throw new InvalidOperationException("SignKey参数必须为24位");

        kernelServices.Services.AddHttpContextAccessor();

        kernelServices.Services.AddAuthentication(r =>
            {
                r.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                r.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                r.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                r.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                r.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                r.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                r.AddScheme<SecretAuthenticationHandler>(SecretAuthenticationHandler.SecretScheme,
                    SecretAuthenticationHandler.SecretScheme);
            })
            .AddJwtBearer(r =>
            {
                r.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Iss,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(RcOptions.Value.SignKey)),
                    ValidateAudience = true,
                    ValidAudience = Aud,
                    SaveSigninToken = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                };
                r.RequireHttpsMetadata = false;
                r.SaveToken = true;
            });

        kernelServices.Services.AddAuthorization();
    }
}