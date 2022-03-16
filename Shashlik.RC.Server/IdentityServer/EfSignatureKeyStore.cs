using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shashlik.RC.Server.Data;
using Shashlik.RC.Server.Data.Entities;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.Server.IdentityServer
{
    public class EfSignatureKeyStore : IValidationKeysStore, ISigningCredentialStore
    {
        public EfSignatureKeyStore(RCDbContext dbContext, ILogger<EfSignatureKeyStore> logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        private RCDbContext DbContext { get; }
        private ILogger<EfSignatureKeyStore> Logger { get; }
        private const string RsaKeyType = "rsa";

        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var key = await DbContext.SignatureKeys.Where(r => r.Enabled).FirstOrDefaultAsync();
            if (key is null)
            {
                using var rsa = RSA.Create(2048);
                var privateKey = rsa.ToPem(true, true);

                key = new SignatureKeys
                {
                    PrivateKey = privateKey,
                    KeyType = RsaKeyType,
                    CreateTime = DateTime.Now.GetLongDate(),
                    Enabled = true
                };

                await DbContext.SignatureKeys.AddAsync(key);
                try
                {
                    await DbContext.SaveChangesAsync();
                    Logger.LogInformation($"new rsa key completed");
                }
                //TODO: ...
                catch (DbUpdateException)
                {
                    // ignore
                }
            }

            var list = new List<SecurityKeyInfo>();
            if (key.KeyType.EqualsIgnoreCase(RsaKeyType))
            {
                var rsa = RSAHelper.FromPem(key.PrivateKey);
                var rsaSecurityKey = new RsaSecurityKey(rsa);
                list.Add(new SecurityKeyInfo
                {
                    Key = rsaSecurityKey,
                    SigningAlgorithm = SecurityAlgorithms.RsaSha256
                });
            }

            return list;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var key = await DbContext.SignatureKeys.Where(r => r.Enabled).FirstOrDefaultAsync();
            if (key is null)
            {
                using var rsa = RSA.Create(2048);
                var privateKey = rsa.ToPem(true, true);
                key = new SignatureKeys
                {
                    PrivateKey = privateKey,
                    KeyType = RsaKeyType,
                    CreateTime = DateTime.Now.GetLongDate(),
                    Enabled = true
                };

                await DbContext.SignatureKeys.AddAsync(key);
                try
                {
                    await DbContext.SaveChangesAsync();
                    Logger.LogInformation($"new rsa key completed");
                }
                //TODO: ...
                catch (DbUpdateException)
                {
                    // ignore
                }
            }

            return new SigningCredentials(new RsaSecurityKey(RSAHelper.FromPem(key.PrivateKey)), "RS256");
        }
    }
}