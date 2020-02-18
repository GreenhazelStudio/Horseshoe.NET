using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.IO.Email
{
    public static class EmailSettings
    {
        private static string _defaultSmtpServer;

        /// <summary>
        /// Gets or sets the default SMTP server used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Smtp.Server and OrganizationalDefaultSettings: key = Smtp.Server)
        /// </summary>
        public static string DefaultSmtpServer
        {
            get
            {
                return _defaultSmtpServer
                    ?? Config.Get("Horseshoe.NET:Smtp.Server") 
                    ?? OrganizationalDefaultSettings.GetString("Smtp.Server");
            }
            set
            {
                _defaultSmtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default SMTP port used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Smtp.Port and OrganizationalDefaultSettings: key = Smtp.Port)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.GetNInt("Horseshoe.NET:Smtp.Port") 
                    ?? OrganizationalDefaultSettings.GetNInt("Smtp.Port");
            }
            set
            {
                _defaultPort = value;
            }
        }

        private static bool? _defaultEnableSsl;

        /// <summary>
        /// Gets or sets the SSL setting used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Smtp.EnableSsl and OrganizationalDefaultSettings: key = Smtp.EnableSsl)
        /// </summary>
        public static bool DefaultEnableSsl
        {
            get
            {
                return _defaultEnableSsl
                    ?? Config.GetNBoolean("Horseshoe.NET:Smtp.EnableSsl")
                    ?? OrganizationalDefaultSettings.GetBoolean("Smtp.EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SMTP login credentials used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Smtp.UserName|Password|IsEncryptedPassword|Domain and OrganizationalDefaultSettings: key = Smtp.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:Smtp.UserName"), Config.Get("Horseshoe.NET:Smtp.Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:Smtp.IsEncryptedPassword"), domain: Config.Get("Horseshoe.NET:Smtp.Domain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Smtp.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultFrom;

        /// <summary>
        /// Gets or sets the default sender address used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email.From and OrganizationalDefaultSettings: key = Email.From)
        /// </summary>
        public static string DefaultFrom
        {
            get
            {
                return _defaultFrom
                    ?? Config.Get("Horseshoe.NET:Email.From")
                    ?? OrganizationalDefaultSettings.GetString("Email.From");
            }
            set
            {
                _defaultFrom = value;
            }
        }

        static string _defaultFooterText;

        /// <summary>
        /// Gets or sets the default footer text used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:FooterText and OrganizationalDefaultSettings: key = Email.Footer)
        /// </summary>
        public static string DefaultFooterText
        {
            get
            {
                return _defaultFooterText
                    ?? Config.Get("Horseshoe.NET:Email.Footer")
                    ?? OrganizationalDefaultSettings.GetString("Email.Footer");
            }
            set
            {
                _defaultFooterText = value;
            }
        }
    }
}
