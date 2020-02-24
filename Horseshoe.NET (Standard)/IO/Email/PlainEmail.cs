using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.Email
{
    public static class PlainEmail
    {
        public static void Send
        (
            string subject,
            string body,
            EmailAddressList to,
            EmailAddressList cc = null,
            EmailAddressList bcc = null,
            string from = null,
            string attach = null,
            IEnumerable<string> attachments = null,
            string footerText = null,
            Encoding encoding = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            // create the mail client
            var smtpClient = SmtpUtil.GetSmtpClient(connectionInfo: connectionInfo);

            // validate and create the mail message
            SmtpUtil.Validate
            (
                body,
                subject,
                to,
                from,
                attach,
                attachments
            );

            var mailMessage = new MailMessage()
            {
                Subject = subject ?? "",
                Body = JoinBodyAndFooter(body ?? "", footerText ?? EmailSettings.DefaultFooterText),
                BodyEncoding = encoding ?? Encoding.ASCII,
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = false
            };

            foreach (var recipient in to)
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            if (cc != null)
            {
                foreach (var recipient in cc)
                {
                    mailMessage.CC.Add(new MailAddress(recipient));
                }
            }

            if (bcc != null)
            {
                foreach (var recipient in bcc)
                {
                    mailMessage.Bcc.Add(new MailAddress(recipient));
                }
            }

            if (attach != null && (attachments == null || !attachments.Any()))
            {
                attachments = attach.Split(',', ';', '|').ZapAndPrune();
            }

            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
            }

            // send mail
            smtpClient.Send(mailMessage);
        }

        private static string JoinBodyAndFooter(string body, string footerText)
        {
            if (footerText == null) return body;
            return body + Environment.NewLine + Environment.NewLine + footerText;
        }
    }
}
