using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Horseshoe.NET.Mvc
{
    public class EnableOriginalRequestBodyResourceFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // do nothing here
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.MethodInfo.CustomAttributes.Any(ca => ca.AttributeType == typeof(EnableOriginalRequestBodyAttribute)))
            {
                context.HttpContext.Request.EnableBuffering();
                var syncIOFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIOFeature != null)
                {
                    syncIOFeature.AllowSynchronousIO = true;
                }
            }
        }
    }
}
