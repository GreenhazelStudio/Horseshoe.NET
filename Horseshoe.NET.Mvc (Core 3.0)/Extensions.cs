using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

using Microsoft.AspNetCore.Http;

namespace Horseshoe.NET.Mvc
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the original http request body as a string.  Note: For this to work remember to follow the setup instructions (see documentation - hint: startup, configure services, add controllers with views, options.Filters.Add(typeof(GetRequestBodyTextResourceFilter)));
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetBodyText(this HttpRequest request)
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

        public static string ToCssClass(this Bootstrap3.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap3.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        public static string ToCssClass(this Bootstrap4.AlertType alertType)
        {
            switch (alertType)
            {
                case Bootstrap4.AlertType.Error:
                    return "alert-danger";
                default:
                    return "alert-" + alertType.ToString().ToLower();
            }
        }

        internal static AlertMessageDetailsRenderingPolicy ToAlertMessageDetailsRendering(this ExceptionRenderingPolicy exceptionRendering)
        {
            switch (exceptionRendering)
            {
                case ExceptionRenderingPolicy.InAlert:
                    return AlertMessageDetailsRenderingPolicy.HtmlEncoded | AlertMessageDetailsRenderingPolicy.PreFormatted;
                case ExceptionRenderingPolicy.InAlertHidden:
                default:
                    return AlertMessageDetailsRenderingPolicy.Default;
            }
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
