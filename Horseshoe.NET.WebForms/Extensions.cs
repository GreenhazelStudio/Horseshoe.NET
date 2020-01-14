using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.WebForms
{
    public static class Extensions
    {
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

        public static Bootstrap3.WebFormsBootstrapAlert ToControl(this Bootstrap3.Alert alert)
        {
            return new Bootstrap3.WebFormsBootstrapAlert(alert);
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

        public static Bootstrap4.WebFormsBootstrapAlert ToControl(this Bootstrap4.Alert alert)
        {
            return new Bootstrap4.WebFormsBootstrapAlert(alert);
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

        public static string GetRemoteIPAddress(this Page page)
        {
            return TextUtil.Zap(page.Request?.UserHostAddress);
        }

        public static string GetRemoteIPAddress(this HttpContext httpContext)
        {
            return TextUtil.Zap(httpContext.Request?.UserHostAddress);
        }

        public static string GetRemoteMachineName(this Page page)
        {
            return TextUtil.Zap(page.Request?.UserHostName);
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
