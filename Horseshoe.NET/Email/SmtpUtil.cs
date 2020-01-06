using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;

using Horseshoe.NET.Cryptography;

namespace Horseshoe.NET.Email
{
    public static class SmtpUtil
    {
        private static CryptoOptions CryptoOptions { get; } = new CryptoOptions
        {
            Algorithm = new System.Security.Cryptography.AesCryptoServiceProvider(),
            KeyText = "4389nP9498NP(*34"
        };

        public static string Encrypt(string plainText)
        {
            var cipherText = Cryptography.Encrypt.String(plainText, CryptoOptions);
            return cipherText;
        }

        public static string Decrypt(string cipherText)
        {
            var reconstitutedPlainText = Cryptography.Decrypt.String(cipherText, CryptoOptions);
            return reconstitutedPlainText;
        }

        public static SecureString DecryptSecure(string cipherText)
        {
            var secureString = Cryptography.Decrypt.SecureString(cipherText, CryptoOptions);
            return secureString;
        }

        public static SmtpClient WhichSmtpClient(SmtpConnectionInfo connectionInfo = null)
        {
            if (connectionInfo != null)
            {
                return connectionInfo.GetSmtpClient();
            }
            else
            {
                return GetDefaultSmtpClient();
            }
        }

        public static SmtpClient GetSmtpClient(SmtpConnectionInfo connectionInfo = null)
        {
            return WhichSmtpClient(connectionInfo: connectionInfo) ?? throw new UtilityException("No SMTP server info was found");
        }

        internal static SmtpClient GetDefaultSmtpClient()
        {
            if (Settings.DefaultSmtpServer == null) return null;
            
            var smtpClient = new SmtpClient(Settings.DefaultSmtpServer);
            if (Settings.DefaultPort.HasValue) smtpClient.Port = Settings.DefaultPort.Value;
            if (Settings.DefaultEnableSsl) smtpClient.EnableSsl = true;
            if (Settings.DefaultCredentials.HasValue)
            {
                if (Settings.DefaultCredentials.Value.HasSecurePassword)
                {
                    smtpClient.Credentials = Settings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(Settings.DefaultCredentials.Value.UserID, Settings.DefaultCredentials.Value.SecurePassword, Settings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(Settings.DefaultCredentials.Value.UserID, Settings.DefaultCredentials.Value.SecurePassword);
                }
                else if (Settings.DefaultCredentials.Value.IsEncryptedPassword)
                {
                    smtpClient.Credentials = Settings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(Settings.DefaultCredentials.Value.UserID, DecryptSecure(Settings.DefaultCredentials.Value.Password), Settings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(Settings.DefaultCredentials.Value.UserID, DecryptSecure(Settings.DefaultCredentials.Value.Password));
                }
                else if (Settings.DefaultCredentials.Value.Password != null)
                {
                    smtpClient.Credentials = Settings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(Settings.DefaultCredentials.Value.UserID, Settings.DefaultCredentials.Value.Password, Settings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(Settings.DefaultCredentials.Value.UserID, Settings.DefaultCredentials.Value.Password);
                }
                else
                {
                    smtpClient.Credentials = Settings.DefaultCredentials.Value.Domain != null
                        ? new NetworkCredential(Settings.DefaultCredentials.Value.UserID, null as string, Settings.DefaultCredentials.Value.Domain)
                        : new NetworkCredential(Settings.DefaultCredentials.Value.UserID, null as string);
                }
            }
            return smtpClient;
        }

        internal static void Validate(EmailInfo emailInfo, SmtpConnectionInfo connectionInfo)
        {
            if (emailInfo == null)
            {
                throw new UtilityException("EmailInfo argument must be non-null");
            }

            if (connectionInfo == null)
            {
                throw new UtilityException("SMTP connection info was not supplied, try adding via config file");
            }

            var validationMessages = new List<string>();
            if (string.IsNullOrEmpty(emailInfo.Subject) && string.IsNullOrEmpty(emailInfo.Body) && (emailInfo.Attachments == null || !emailInfo.Attachments.Any()))
            {
                validationMessages.Add("Email may not be devoid of Subject, Body and Attachments (it must contain at least one of these)");
            }

            if (emailInfo.Tos == null || !emailInfo.Tos.Any())
            {
                validationMessages.Add("Email must be sent to at least one [To] recipient");
            }

            if (validationMessages.Any())
            {
                throw new ValidationException { ValidationMessages = validationMessages.ToArray() };
            }
        }
    }
}
