using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Horseshoe.NET.Text;

using Microsoft.AspNetCore.Http;

namespace Horseshoe.NET.Mvc
{
    public static class Extensions
    {
        public static string GetOriginalRequestBody(this HttpRequest request)
        {
            var streamReader = new StreamReader(request.Body);
            return streamReader.ReadToEnd();
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
