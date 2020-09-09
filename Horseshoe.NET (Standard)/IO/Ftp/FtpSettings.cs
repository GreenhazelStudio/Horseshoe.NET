using System;

namespace Horseshoe.NET.IO.Ftp
{
    public static class FtpSettings
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
                    ?? OrganizationalDefaultSettings.GetString("Ftp.Server");
            }
            set
            {
                _defaultFtpServer = value;
            }
        }

        private static bool? _defaultEnableSsl;

        /// <summary>
        /// Gets or sets whether FTP will use SSL.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp.EnableSsl and OrganizationalDefaultSettings: key = Ftp.EnableSsl)
        /// </summary>
        public static bool DefaultEnableSsl
        {
            get
            {
                return _defaultEnableSsl
                    ?? OrganizationalDefaultSettings.GetBoolean("Ftp.EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
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
                    ?? OrganizationalDefaultSettings.GetNInt("Ftp.Port");
            }
            set
            {
                _defaultPort = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default credentials used by FTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp.UserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Ftp.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Ftp.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultServerPath;

        /// <summary>
        /// Gets or sets the default server path used by FTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Ftp.ServerPath and OrganizationalDefaultSettings: key = Ftp.ServerPath)
        /// </summary>
        public static string DefaultServerPath
        {
            get
            {
                return _defaultServerPath
                    ?? "";
            }
            set
            {
                _defaultServerPath = value;
            }
        }
    }
}
