using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shashlik.RC.Controllers
{
    public class ServerController : ApiControllerBase
    {
        /// <summary>
        /// 配置刷新通知
        /// </summary>
        /// <param name="serverToken"></param>
        /// <param name="appId"></param>
        /// <param name="env"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        [HttpGet("refresh")]
        [AllowAnonymous]
        public async Task RefreshNotify(string serverToken, string appId, string env, string configName)
        {
            //TODO: ..
        }
    }
}