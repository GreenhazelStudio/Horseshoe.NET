﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.WebForms
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