using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shashlik.Utils.Extensions;

// ReSharper disable InvertIf

namespace Shashlik.RC.Filters
{
    /// <summary>
    /// 自动包装结果为<see cref="ResponseResult"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ResponseWrapperAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 是否将模型验证错误转换位http 200 ,默认ture
        /// </summary>
        public bool ModelError2HttpOk { get; set; } = true;

        /// <summary>
        /// 是否输出所有的模型验证错误, 默认false
        /// </summary>
        public bool ResponseAllModelError { get; set; }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
                {
                    if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                        || actionDescriptor.MethodInfo.DeclaringType!.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                       )
                    {
                        await base.OnResultExecutionAsync(context, next);
                        return;
                    }
                }

                context.HttpContext.Response.StatusCode =
                    ModelError2HttpOk ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;

                string? error;
                if (!ResponseAllModelError)
                    error = context.ModelState
                        .SelectMany(r => r.Value?.Errors ?? new ModelErrorCollection())
                        .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.ErrorMessage))
                        ?.ErrorMessage;
                else
                {
                    error = context.ModelState
                        .SelectMany(r => r.Value?.Errors ?? new ModelErrorCollection())
                        .Select(r => r.ErrorMessage)
                        .Join(Environment.NewLine);
                }

                context.Result = new ObjectResult(new ResponseResult(400, false,
                    error ?? "invalid request", null, null));
            }

            await base.OnResultExecutionAsync(context, next);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.MethodInfo.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                    || actionDescriptor.MethodInfo.DeclaringType!.IsDefinedAttribute<NoResponseWrapperAttribute>(true)
                   )
                {
                    base.OnActionExecuted(context);
                    return;
                }


                base.OnActionExecuted(context);
                switch (context.Result)
                {
                    case EmptyResult _:
                        context.Result = new ObjectResult(new ResponseResult(200, true,
                            "success", null, null));
                        break;
                    case ObjectResult result:
                    {
                        if (result.DeclaredType != typeof(ResponseResult))
                            result.Value = new ResponseResult(200, true,
                                "success", result.Value, null);

                        break;
                    }
                    case ContentResult contentResult:
                        context.Result = new ObjectResult(new ResponseResult(200, true,
                            "success", contentResult.Content, null));
                        break;
                }
            }
        }
    }
}