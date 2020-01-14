using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Mvc
{
    public static class Extensions
    {
        public static string GetBodyText(this HttpRequestBase request)
        {
            using (var stream = new MemoryStream())
            {
                request.InputStream.CopyTo(stream);
                var rawBody = (request.ContentEncoding ?? Encoding.UTF8).GetString(stream.ToArray());
                return rawBody;
            }
        }

        public static string GetAbsoluteApplicationPath(this HttpRequestBase request, string virtualSubpath = null, bool excludeQueryString = false)
        {
            var uri = request.Url;
            var sb = new StringBuilder(uri.Scheme)       // http
                .Append("://")
                .Append(uri.Host)                        // dev-web01.dev.local
                .AppendIf
                (
                    uri.Port != 0 && !(uri.Scheme.ToLower() + uri.Port).In("http80", "https443"),
                    ":" + uri.Port                       // :8080
                )
                .Append(request.ApplicationPath);        // /test_props

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

            if (!excludeQueryString && uri.Query.Length > 0)
            {
                if (virtualSubpath == null && request.ApplicationPath.Length == 0)
                {
                    sb.Append("/");
                }
                sb.Append(uri.Query);
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
            return TextUtil.Zap(httpContext.Request?.UserHostAddress);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.Request?.UserHostName);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.User?.Identity.Name) ?? TextUtil.Zap(httpContext.Request?.ServerVariables["UNMAPPED_REMOTE_USER"]);
        }
    }
}
