﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shashlik.RC.EventBus;
using Shashlik.RC.Filters;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ServerTokenFilter]
    public class ServerController : ControllerBase
    {
        public ServerController(IMediator mediator)
        {
            Mediator = mediator;
        }

        private IMediator Mediator { get; }

        /// <summary>
        /// 内部事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventJsonData"></param>
        /// <returns></returns>
        [HttpPost("event")]
        [AllowAnonymous]
        public async Task<IActionResult> Event([FromHeader, Required] string eventType, [FromBody, Required] string eventJsonData)
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
                HttpContext.RequestServices.GetRequiredService<ILogger<ServerController>>()
                    .LogDebug(ex, $"inner server event handling occurred error");
                return BadRequest();
            }
        }
    }
}