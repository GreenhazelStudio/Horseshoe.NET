using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Email
{
    public static class Settings
    {
        private static string _defaultSmtpServer;

        /// <summary>
        /// Gets or sets the default SMTP server used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpServer and OrganizationalDefaultSettings: key = Email.SmtpServer)
        /// </summary>
        public static string DefaultSmtpServer
        {
            get
            {
                return _defaultSmtpServer
                    ?? Config.Get("Horseshoe.NET:Email:SmtpServer") 
                    ?? OrganizationalDefaultSettings.GetString("Email.SmtpServer");
            }
            set
            {
                _defaultSmtpServer = value;
            }
        }

        private static int? _defaultPort;

        /// <summary>
        /// Gets or sets the default SMTP port used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:SmtpPort and OrganizationalDefaultSettings: key = Email.SmtpPort)
        /// </summary>
        public static int? DefaultPort
        {
            get
            {
                return _defaultPort
                    ?? Config.GetNInt("Horseshoe.NET:Email:SmtpPort") 
                    ?? OrganizationalDefaultSettings.GetNInt("Email.SmtpPort");
            }
            set
            {
                _defaultPort = value;
            }
        }

        private static bool? _defaultEnableSsl;

        /// <summary>
        /// Gets or sets the SSL setting used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:EnableSsl and OrganizationalDefaultSettings: key = Email.EnableSsl)
        /// </summary>
        public static bool DefaultEnableSsl
        {
            get
            {
                return _defaultEnableSsl
                    ?? Config.GetNBoolean("Horseshoe.NET:Email:EnableSsl")
                    ?? OrganizationalDefaultSettings.GetBoolean("Email.EnableSsl");
            }
            set
            {
                _defaultEnableSsl = value;
            }
        }

        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets the default SMTP login credentials used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:UserName|Password|IsEncryptedPassword and OrganizationalDefaultSettings: key = Email.Credentials)
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:Email:UserName"), Config.Get("Horseshoe.NET:Email:Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:Email:IsEncryptedPassword"), domain: Config.Get("Horseshoe.NET:Email:UserDomain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("Email.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }

        static string _defaultFrom;

        /// <summary>
        /// Gets or sets the default sender address used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:From and OrganizationalDefaultSettings: key = Email.From)
        /// </summary>
        public static string DefaultFrom
        {
            get
            {
                return _defaultFrom
                    ?? Config.Get("Horseshoe.NET:Email:From")
                    ?? OrganizationalDefaultSettings.GetString("Email.From");
            }
            set
            {
                _defaultFrom = value;
            }
        }

        static string _defaultFooterText;

        /// <summary>
        /// Gets or sets the default footer text used by PlainEmail and HtmlEmail.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email:FooterText and OrganizationalDefaultSettings: key = Email.FooterText)
        /// </summary>
        public static string DefaultFooterText
        {
            get
            {
                return _defaultFooterText
                    ?? Config.Get("Horseshoe.NET:Email:FooterText")
                    ?? OrganizationalDefaultSettings.GetString("Email.FooterText");
            }
            set
            {
                _defaultFooterText = value;
            }
        }
    }
}
