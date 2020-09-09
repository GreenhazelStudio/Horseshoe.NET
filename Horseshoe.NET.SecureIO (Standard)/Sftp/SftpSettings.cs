using System;

namespace Horseshoe.NET.SecureIO.Sftp
{
    public static class SftpSettings
    {
        private static string _defaultFtpServer;

        /// <summary>
        /// Gets or sets the default SFTP server.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp.Server and OrganizationalDefaultSettings: key = Sftp.Server)
        /// </summary>
        public static string DefaultFtpServer
        {
            get
            {
                return _defaultFtpServer
                    ?? OrganizationalDefaultSettings.GetString("Sftp.Server");
            }
            set
            {
                _defaultFtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default SFTP port.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp.Port and OrganizationalDefaultSettings: key = Sftp.Port)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? OrganizationalDefaultSettings.GetNInt("Sftp.Port");
            }
            set
            {
                _defaultPort = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default credentials used by SFTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp.UserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Sftp.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Sftp.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultServerPath;

        /// <summary>
        /// Gets or sets the default server path used by SFTP.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Sftp.ServerPath and OrganizationalDefaultSettings: key = Sftp.ServerPath)
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
