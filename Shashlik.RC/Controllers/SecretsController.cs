using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shashlik.RC.Services.Secret;
using Shashlik.RC.Services.Secret.Dtos;

namespace Shashlik.RC.Controllers
{
    public class SecretsController : ApiControllerBase
    {
        public SecretsController(SecretService secretService)
        {
            SecretService = secretService;
        }

        private SecretService SecretService { get; }

        [HttpGet]
        public async Task<List<SecretDto>> Get()
        {
            return await SecretService.List(LoginUserId.ToString()!);
        }

        [HttpPost]
        public async Task Post()
        {
            await SecretService.Create(LoginUserId.ToString()!);
        }

        [HttpDelete("{secretId:length(32)}")]
        public async Task Delete(string secretId)
        {
            await SecretService.Delete(LoginUserId.ToString()!, secretId);
        }
    }
}