using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Http;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Mvc.Extensions
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

        public static string GetAbsoluteApplicationPath(this HttpRequest request, string virtualSubpath = null, bool includeQueryString = false, string overrideScheme = null, string overrideHost = null, int? overridePort = null, string overridePathBase = null)
        {
            var sb = new StringBuilder(overrideScheme ?? request.Scheme)   // http
                .Append("://")
                .Append(overrideHost == null ? request.Host.ToString() : overrideHost + (overridePort.HasValue ? ":" + overridePort : ""))    // dev-web01.dev.local:8080
                .Append(overridePathBase ?? request.PathBase);             // /test_props

            if (virtualSubpath != null)                                    // /api
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
                if (virtualSubpath == null && overridePathBase == null && !request.PathBase.HasValue)
                {
                    sb.Append("/");
                }
                sb.Append(request.QueryString);
            }

            return sb.ToString();
        }

        public static string GetRemoteIPAddress(this HttpRequest request)
        {
            return GetRemoteIPAddress(request.HttpContext);
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return Zap.String(httpContext.Connection.RemoteIpAddress);
        }

        public static string GetRemoteMachineName(this HttpRequest request)
        {
            return GetRemoteMachineName(request.HttpContext);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            try
            {
                return Dns.GetHostEntry(httpContext.Connection.RemoteIpAddress).HostName;
            }
            catch (SocketException)
            {
                return null;
            }
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return Zap.String(httpContext.User?.Identity.Name);
        }
    }
}
