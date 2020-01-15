using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Horseshoe.NET.Mvc
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the original http request body as a string.  Note: For this to work you must tag the controller action with the [EnableOriginalRequestBody] attribute and do the setup (see documentation - hint: Startup, ConfigureServices, AddControllersWithViews(options.Filters.Add<EnableOriginalRequestBodyResourceFilter>()))
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetOriginalRequestBody(this HttpRequest request)
        {
            var bodyStream = new StreamReader(request.Body, leaveOpen: true);
            var bodyStreamResult = bodyStream.ReadToEnd();
            return bodyStreamResult;
        }

        public static string GetAbsoluteApplicationPath(this HttpRequest request, string virtualSubpath = null, bool includeQueryString = false)
        {
            var sb = new StringBuilder(request.Scheme)   // http
                .Append("://")
                .Append(request.Host)                    // dev-web01.dev.local:8080
                .Append(request.PathBase);               // /test_props

            if (virtualSubpath != null)                  // /api
            {
                if (!sb.ToString().EndsWith("/"))
                {
                    sb.Append("/");
                }
                if (virtualSubpath.StartsWith("/"))
                {
                    sb.Append(virtualSubpath.Substring(1));
                }
                else
                {
                    sb.Append(virtualSubpath);
                }
            }

            if (includeQueryString && request.QueryString.HasValue)
            {
                if (virtualSubpath == null && !request.PathBase.HasValue)
                {
                    sb.Append("/");
                }
                sb.Append(request.QueryString);
            }

            return sb.ToString();
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.Connection.RemoteIpAddress);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.Request.Host.Host);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.User?.Identity.Name);
        }
    }
}
