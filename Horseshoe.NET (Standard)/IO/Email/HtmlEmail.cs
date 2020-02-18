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
            string body,
            string subject,
            string to = null,
            IEnumerable<string> recipients = null,
            string cc = null,
            IEnumerable<string> ccRecipients = null,
            string bcc = null,
            IEnumerable<string> bccRecipients = null,
            string from = null,
            string attach = null,
            IEnumerable<string> attachments = null,
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
                recipients,
                cc,
                ccRecipients,
                bcc,
                bccRecipients,
                from,
                attach,
                attachments
            );

            var mailMessage = new MailMessage()
            {
                Subject = subject ?? "",
                Body = JoinBodyAndFooter(body ?? "", footerHtml ?? EmailSettings.DefaultFooterText),
                BodyEncoding = encoding ?? Encoding.UTF8,
                From = new MailAddress(from ?? EmailSettings.DefaultFrom),
                IsBodyHtml = true
            };

            if (to != null && (recipients == null || !recipients.Any()))
            {
                recipients = to.Split(',', ';').ZapAndPrune();
            }

            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            if (cc != null && (ccRecipients == null || !ccRecipients.Any()))
            {
                ccRecipients = cc.Split(',', ';').ZapAndPrune();
            }

            if (ccRecipients != null && ccRecipients.Any())
            {
                foreach (var recipient in ccRecipients)
                {
                    mailMessage.To.Add(new MailAddress(recipient));
                }
            }

            if (bcc != null && (bccRecipients == null || !bccRecipients.Any()))
            {
                bccRecipients = bcc.Split(',', ';').ZapAndPrune();
            }

            if (bccRecipients != null && bccRecipients.Any())
            {
                foreach (var recipient in bccRecipients)
                {
                    mailMessage.To.Add(new MailAddress(recipient));
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
