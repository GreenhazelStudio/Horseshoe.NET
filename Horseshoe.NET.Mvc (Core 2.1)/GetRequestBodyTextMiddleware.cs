using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Horseshoe.NET.Mvc
{
    // ref https://stackoverflow.com/questions/40494913/how-to-read-request-body-in-a-asp-net-core-webapi-controller
    public class GetRequestBodyTextMiddleware
    {
        private readonly RequestDelegate _next;

        public GetRequestBodyTextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableRewind();
            await _next(context);
        }
    }
}
