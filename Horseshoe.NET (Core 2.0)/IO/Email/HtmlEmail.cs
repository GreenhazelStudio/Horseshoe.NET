using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.IO.Email
{
    public static class HtmlEmail
    {
        public static void Send
        (
            string subject,
            string body,
            StringList to,
            StringList cc = null,
            StringList bcc = null,
            string from = null,
            StringList attach = null,
            string footerHtml = null,
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
                attach
            );

            var mailMessage = new MailMessage()
            {
                Subject = subject ?? "",
                Body = JoinBodyAndFooter(body ?? "", footerHtml ?? EmailSettings.DefaultFooterText),
                BodyEncoding = encoding ?? Encoding.UTF8,
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = true
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

            if (attach != null && attach.Any())
            {
                foreach (var attachment in attach)
                {
                    mailMessage.Attachments.Add(new Attachment(attachment));
                }
            }

            // send mail
            smtpClient.Send(mailMessage);
        }

        private static string JoinBodyAndFooter(string body, string footerHtml)
        {
            if (footerHtml == null) return body;
            var oIndex = body.ToLower().IndexOf("<body");
            var cIndex = body.ToLower().IndexOf("</body>");
            if (cIndex > oIndex && oIndex >= 0)
            {
                var sb = new StringBuilder(body);
                sb.Insert(cIndex, "<br /><br />" + footerHtml);
                return sb.ToString();
            }
            else
            {
                return body + "<br /><br />" + footerHtml;
            }
        }
    }
}
