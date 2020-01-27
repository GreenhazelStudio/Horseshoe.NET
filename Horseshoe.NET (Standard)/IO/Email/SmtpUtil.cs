using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;

using Horseshoe.NET.Cryptography;

namespace Horseshoe.NET.IO.Email
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

        internal static void Validate
        (
            string subject,
            string body,
            string to = null,
            IEnumerable<string> recipients = null,
            string cc = null,
            IEnumerable<string> ccRecipients = null,
            string bcc = null,
            IEnumerable<string> bccRecipients = null,
            string from = null,
            string attach = null,
            IEnumerable<string> attachments = null
        )
        {
            var validationMessages = new List<string>();

            if (to == null && (recipients == null || !recipients.Any()))
            {
                validationMessages.Add("Please populate 'to' or 'recipients', mail cannot be sent otherwise");
            }

            if (to != null && !(recipients == null || !recipients.Any()))
            {
                validationMessages.Add("Please only populate 'to' or 'recipients', not both");
            }

            if (cc != null && !(ccRecipients == null || !ccRecipients.Any()))
            {
                validationMessages.Add("Please only populate 'cc' or 'ccRecipients', not both");
            }

            if (bcc != null && !(bccRecipients == null || !bccRecipients.Any()))
            {
                validationMessages.Add("Please only populate 'bcc' or 'bccRecipients', not both");
            }

            if (from == null && Settings.DefaultFrom == null)
            {
                validationMessages.Add("Please supply a 'from' address, you may configure this value (key=\"Horseshoe.NET:Email:From\")");
            }

            if (attach != null && !(attachments == null || !attachments.Any()))
            {
                validationMessages.Add("Please only populate 'attach' or 'attachments', not both");
            }

            if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(body) && attach == null && (attachments == null || !attachments.Any()))
            {
                validationMessages.Add("Email may preclude 'subject', 'body' or 'attachments' but not all three");
            }

            if (validationMessages.Any())
            {
                throw new ValidationException { ValidationMessages = validationMessages.ToArray() };
            }
        }
    }
}
