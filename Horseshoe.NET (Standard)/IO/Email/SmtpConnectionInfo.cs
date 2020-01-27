using System.Net;
using System.Net.Mail;

namespace Horseshoe.NET.IO.Email
{
    public class SmtpConnectionInfo
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public Credential? Credentials { get; set; }
        public bool EnableSsl { get; set; }

        internal SmtpClient GetSmtpClient()
        {
            if (Server == null) return null;

            var smtpClient = new SmtpClient(Server);
            if (Port.HasValue) smtpClient.Port = Port.Value;
            if (EnableSsl) smtpClient.EnableSsl = true;

            if (Credentials.HasValue)
            {
                if (Credentials.Value.HasSecurePassword)
                {
                    smtpClient.Credentials = Credentials.Value.Domain != null
                        ? new NetworkCredential(Credentials.Value.UserID, Credentials.Value.SecurePassword, Credentials.Value.Domain)
                        : new NetworkCredential(Credentials.Value.UserID, Credentials.Value.SecurePassword);
                }
                else if (Credentials.Value.IsEncryptedPassword)
                {
                    smtpClient.Credentials = Credentials.Value.Domain != null
                        ? new NetworkCredential(Credentials.Value.UserID, SmtpUtil.DecryptSecure(Credentials.Value.Password), Credentials.Value.Domain)
                        : new NetworkCredential(Credentials.Value.UserID, SmtpUtil.DecryptSecure(Credentials.Value.Password));
                }
                else if (Credentials.Value.Password != null)
                {
                    smtpClient.Credentials = Credentials.Value.Domain != null
                        ? new NetworkCredential(Credentials.Value.UserID, Credentials.Value.Password, Credentials.Value.Domain)
                        : new NetworkCredential(Credentials.Value.UserID, Credentials.Value.Password);
                }
                else
                {
                    smtpClient.Credentials = Credentials.Value.Domain != null
                        ? new NetworkCredential(Credentials.Value.UserID, null as string, Credentials.Value.Domain)
                        : new NetworkCredential(Credentials.Value.UserID, null as string);
                }
            }
            return smtpClient;
        }
    }
}
