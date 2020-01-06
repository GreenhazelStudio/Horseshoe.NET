using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.IO.ReportingServices
{
    public static class Settings
    {
        static string _defaultReportServer;

        /// <summary>
        /// Gets or sets the default Report Server used by ReportingServices.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:ReportingServices:ReportServer and OrganizationalDefaultSettings: key = ReportingServices.ReportServer)
        /// </summary>
        public static string DefaultReportServer
        {
            get
            {
                return _defaultReportServer
                    ?? Config.Get("Horseshoe.NET:ReportingServices:ReportServer")
                    ?? OrganizationalDefaultSettings.GetString("ReportingServices.ReportServer");
            }
            set
            {
                _defaultReportServer = value;
            }
        }
    }
}
