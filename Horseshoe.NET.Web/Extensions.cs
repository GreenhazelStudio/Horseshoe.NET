using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Web
{
    public static class Extensions
    {
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
