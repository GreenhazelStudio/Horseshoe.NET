using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.IO.Ftp
{
    public static class Settings
    {
        private static string _defaultFtpServer;

        /// <summary>
        /// Gets or sets the default FTP server.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp.Server and OrganizationalDefaultSettings: key = Ftp.Server)
        /// </summary>
        public static string DefaultFtpServer
        {
            get
            {
                return _defaultFtpServer
                    ?? Config.Get("Horseshoe.NET:Ftp.Server") 
                    ?? OrganizationalDefaultSettings.GetString("Ftp.Server");
            }
            set
            {
                _defaultFtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default FTP port.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp.Port and OrganizationalDefaultSettings: key = Ftp.Port)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.GetNInt("Horseshoe.NET:Ftp.Port") 
                    ?? OrganizationalDefaultSettings.GetNInt("Ftp.Port");
            }
            set
            {
                _defaultPort = value;
            }
        }
    }
}
