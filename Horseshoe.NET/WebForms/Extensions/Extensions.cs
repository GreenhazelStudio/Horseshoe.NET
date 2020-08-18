using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using Horseshoe.NET.Bootstrap;
using static Horseshoe.NET.ObjectClean.Methods;

namespace Horseshoe.NET.WebForms.Extensions
{
    public static class Extensions
    {
        public static WebFormsBootstrap3Alert ToControl(this Bootstrap3.Alert alert)
        {
            return new WebFormsBootstrap3Alert(alert);
        }

        public static WebFormsBootstrap4Alert ToControl(this Bootstrap4.Alert alert)
        {
            return new WebFormsBootstrap4Alert(alert);
        }

        public static string GetRemoteIPAddress(this Page page)
        {
            return ZapString(page.Request?.UserHostAddress);
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return ZapString(httpContext.Request?.UserHostAddress);
        }

        public static string GetRemoteMachineName(this Page page)
        {
            return ZapString(page.Request?.UserHostName);
        }

        public static string GetRemoteMachineName(this HttpContext httpContext)
        {
            return ZapString(httpContext.Request?.UserHostName);
        }

        public static string GetUserName(this HttpContext httpContext)
        {
            return ZapString(httpContext.User?.Identity.Name) ?? ZapString(httpContext.Request?.ServerVariables["UNMAPPED_REMOTE_USER"]);
        }
    }
}
