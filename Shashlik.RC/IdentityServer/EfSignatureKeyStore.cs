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
using Shashlik.RC.Data;
using Shashlik.RC.Data.Entities;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.RC.IdentityServer
{
    public class EfSignatureKeyStore : IValidationKeysStore
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
            var keys = await DbContext.SignatureKeys.Where(r => r.Enabled).ToListAsync();
            if (DbContext.SignatureKeys.IsNullOrEmpty())
            {
                using var rsa = RSA.Create(2048);
                var bytes = rsa.ExportRSAPrivateKey();
                var privateKey = Convert.ToBase64String(bytes);
                var key = new SignatureKeys
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
                    keys.Add(key);
                    Logger.LogInformation($"new rsa key completed");
                }
                //TODO: ...
                catch (DbUpdateException)
                {
                    // ignore
                }
            }

            var list = new List<SecurityKeyInfo>();
            foreach (var key in keys)
            {
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
            }

            return list;
        }
    }
}