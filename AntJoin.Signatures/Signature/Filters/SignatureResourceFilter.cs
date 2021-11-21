using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace AntJoin.Signatures
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class SignatureAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ISignatureHandler _handler;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handler"></param>
        public SignatureAttribute(ISignatureHandler handler)
        {
            _handler = handler;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var attribute = descriptor.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true);
            attribute = attribute ?? descriptor.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>();

            if (attribute != null || await _handler.HandleAsync(context.HttpContext))
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
