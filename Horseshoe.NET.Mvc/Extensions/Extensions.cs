using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET.Extensions;
using Horseshoe.NET.Objects.Clean;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.Extensions;

namespace Horseshoe.NET.Mvc.Extensions
{
    public static class Extensions
    {
        public static string GetOriginalRequestBody(this HttpRequestBase request)
        {
            using (var stream = new MemoryStream())
            {
                request.InputStream.CopyTo(stream);
                var rawBody = (request.ContentEncoding ?? Encoding.UTF8).GetString(stream.ToArray());
                return rawBody;
            }
        }

        public static string GetAbsoluteApplicationPath(this HttpRequestBase request, string virtualSubpath = null, bool includeQueryString = false, string overrideScheme = null, string overrideHost = null, int? overridePort = null, string overrideApplicationPath = null)
        {
            var uri = request.Url;
            var sb = new StringBuilder(overrideScheme ?? uri.Scheme)          // http
                .Append("://")
                .Append(overrideHost ?? uri.Host)                             // dev-web01.dev.local
                .AppendIf
                (
                    overridePort.HasValue,
                    ":" + overridePort                                        // :8080
                )
                .AppendIf
                (
                    !overridePort.HasValue && uri.Port != 0 && !((overrideScheme ?? uri.Scheme).ToLower() + uri.Port).In("http80", "https443"),
                    ":" + uri.Port                                            // :8080
                )
                .Append(overrideApplicationPath ?? request.ApplicationPath);  // /test_props

            if (virtualSubpath != null)                                       // /api
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

            if (includeQueryString && uri.Query.Length > 0)
            {
                if (virtualSubpath == null && (overrideApplicationPath ?? request.ApplicationPath).Length == 0)
                {
                    sb.Append("/");
                }
                sb.Append(uri.Query);
            }

            return sb.ToString();
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return GetRemoteIPAddress(httpContext.Request);
        }

        public static string GetRemoteIPAddress(this HttpRequest request)
        {
            return Zap.String(request.UserHostAddress);
        }

        public static string GetRemoteIPAddress(this HttpRequestBase request)
        {
            return Zap.String(request.UserHostAddress);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            return GetRemoteMachineName(httpContext.Request);
        }

        public static string GetRemoteMachineName(this HttpRequest request)
        {
            if (string.Equals(request.UserHostName, GetRemoteIPAddress(request))) return null;
            return Zap.String(request.UserHostName);
        }

        public static string GetRemoteMachineName(this HttpRequestBase request)
        {
            if (string.Equals(request.UserHostName, GetRemoteIPAddress(request))) return null;
            return Zap.String(request.UserHostName);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return Zap.String(httpContext.User?.Identity.Name) ?? Zap.String(httpContext.Request?.ServerVariables["UNMAPPED_REMOTE_USER"]);
        }
    }
}
