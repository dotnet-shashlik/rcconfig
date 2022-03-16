using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.EventBus;
using Shashlik.RC.Server.Filters;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ServerTokenFilter]
    public class InnerServerController : ControllerBase
    {
        public InnerServerController(IMediator mediator)
        {
            Mediator = mediator;
        }

        private IMediator Mediator { get; }

        /// <summary>
        /// 内部事件
        /// </summary>
        /// <param name="eventJsonData"></param>
        /// <returns></returns>
        [HttpPost("event")]
        [AllowAnonymous]
        public async Task<IActionResult> Event([FromBody, Required] string eventJsonData)
        {
            if (Request.Headers.TryGetValue(Constants.HeaderKeys.EventType, out var eventType)
                && !eventType.IsNullOrEmpty())
            {
                try
                {
                    var type = GetType().Assembly.GetType(eventType);
                    if (type is null || !type.IsSubType<IInnerServerEvent>())
                        return BadRequest();
                    var eventInstance = JsonConvert.DeserializeObject(eventJsonData, type);
                    if (eventInstance is null)
                        return BadRequest();
                    await Mediator.Publish(eventInstance);
                    return Ok();
                }
                catch (Exception ex)
                {
                    HttpContext.RequestServices.GetRequiredService<ILogger<InnerServerController>>()
                        .LogDebug(ex, $"inner server event handling occurred error");
                    return BadRequest();
                }
            }

            return BadRequest();
        }
    }
}