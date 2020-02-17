using System.Net;
using System.Net.Mail;

using Horseshoe.NET.Cryptography;

namespace Horseshoe.NET.IO.Email
{
    public class SmtpConnectionInfo
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public Credential? Credentials { get; set; }
        public bool EnableSsl { get; set; }

        internal SmtpClient GetSmtpClient(CryptoOptions options = null)
        {
            if (Server == null) return null;

            var smtpClient = new SmtpClient(Server);

            if (Port.HasValue) smtpClient.Port = Port.Value;

            if (EnableSsl) smtpClient.EnableSsl = true;

            if (Credentials.HasValue)
            {
                smtpClient.Credentials = Credentials.Value.ToNetworkCredentials();
            }
            return smtpClient;
        }
    }
}
