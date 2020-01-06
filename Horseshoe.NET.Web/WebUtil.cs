using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Horseshoe.NET.Application;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Web
{
    public static class WebUtil
    {
        static HttpContext HttpContext { get; } = HttpContext.Current;

        /// <summary>
        /// Gets the IP address of the machine hosting this app
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            return ClientApp.GetIPAddress();
        }

        /// <summary>
        /// Gets the IP address of the remote machine accessing this app
        /// </summary>
        /// <returns></returns>
        public static string GetRemoteIPAddress()
        {
            return TextUtil.Zap(HttpContext?.Request.UserHostAddress);
        }

        /// <summary>
        /// Gets the name of the remote machine accessing this app
        /// </summary>
        /// <returns></returns>
        public static string GetRemoteMachineName()
        {
            return TextUtil.Zap(HttpContext?.Request.UserHostName);
        }

        /// <summary>
        /// Gets the logged in user name
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            return TextUtil.Zap(HttpContext?.User.Identity.Name) ?? TextUtil.Zap(HttpContext?.Request.ServerVariables["UNMAPPED_REMOTE_USER"]);
        }
    }
}
