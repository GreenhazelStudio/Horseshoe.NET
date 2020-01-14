using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Horseshoe.NET.Mvc
{
    public class GetRequestBodyTextResourceFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            // do nothing here
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.Filters.Any(f => f.GetType() == typeof(AllowGetRequestBodyTextAttribute)))
            {
                context.HttpContext.Request.EnableBuffering();
            }
        }
    }
}
