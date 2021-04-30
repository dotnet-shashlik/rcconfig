using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.RC.Common;

namespace Shashlik.RC.IdentityServer
{
    public static class Ids4Extensions
    {
        public const string Client = "shashlik-rc-ui";
        public const string Api = "shashlik-rc-api";
        public const string PasswordGrantType = "password";

        public static void UserIds4(this IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddInMemoryClients(new List<Client>
                {
                    new()
                    {
                        AccessTokenLifetime = SystemEnvironmentUtils.AccessTokenLifetime,
                        AccessTokenType = AccessTokenType.Jwt,
                        AllowedGrantTypes = {PasswordGrantType},
                        AllowedScopes = {Api},
                        ClientId = Client,
                        ClientName = Client,
                        Enabled = true,
                        RequireClientSecret = false,
                    }
                })
                .AddInMemoryApiScopes(new List<ApiScope>
                {
                    new()
                    {
                        DisplayName = Api,
                        Enabled = true,
                        Name = Api,
                        Required = true
                    }
                })
                .AddAspNetIdentity<IdentityUser<int>>()
                .AddProfileService<ProfileService>()
                ;
        }
    }
}