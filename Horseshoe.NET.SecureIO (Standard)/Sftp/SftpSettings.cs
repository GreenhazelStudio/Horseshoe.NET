﻿using System;

using Horseshoe.NET.Application;

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
                    ?? Config.Get("Horseshoe.NET:Sftp.Server") 
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
                    ?? Config.GetNInt("Horseshoe.NET:Sftp.Port") 
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
                    ?? Credential.Build(Config.Get("Horseshoe.NET:Sftp.UserName"), Config.Get("Horseshoe.NET:Sftp.Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:Sftp.IsEncryptedPassword"))
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
                    ?? Config.Get("Horseshoe.NET:Sftp.ServerPath")
                    ?? "";
            }
            set
            {
                _defaultServerPath = value;
            }
        }
    }
}
