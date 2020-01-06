using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Http;

namespace Horseshoe.NET.Web
{
    public static class Extensions
    {
        public static string GetAbsoluteApplicationPath(this HttpRequest request, string virtualSubpath = null, bool excludeQueryString = false)
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

            if (!excludeQueryString && request.QueryString.HasValue)
            {
                if (virtualSubpath == null && !request.PathBase.HasValue)
                {
                    sb.Append("/");
                }
                sb.Append(request.QueryString);
            }

            return sb.ToString();
        }

        public static string RenderHtml(this Exception ex, bool displayFullClassName = false, bool displayMessage = true, bool displayStackTrace = false, int indent = 2, bool recursive = false)
        {
            return ToHtml
            (
                ex.Render(displayFullClassName: displayFullClassName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive)
            );
        }

        public static string RenderHtml(this ExceptionInfo exceptionInfo, bool displayFullClassName = false, bool displayMessage = true, bool displayStackTrace = false, int indent = 2, bool recursive = false)
        {
            return ToHtml
            (
                exceptionInfo.Render(displayFullClassName: displayFullClassName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive)
            );
        }

        public static string ToHtml(this string text)
        {
            return HttpUtility.HtmlEncode(text).Replace("\r\n", "<br />").Replace("\n", "<br />");
        }
    }
}
