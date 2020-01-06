﻿using System;
using System.Net.Mail;
using System.Text;

namespace Horseshoe.NET.Email
{
    public static class PlainEmail
    {
        public static void Send(EmailInfo email, SmtpConnectionInfo connectionInfo = null)
        {
            // create the mail client
            var smtpClient = SmtpUtil.GetSmtpClient(connectionInfo: connectionInfo);

            // validate and create the mail message
            SmtpUtil.Validate(email, connectionInfo);

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(email.From ?? Settings.DefaultFrom),
                Subject = email.Subject,
                Body = JoinBodyAndFooterText(email.Body ?? "", email.FooterText ?? Settings.DefaultFooterText),
                BodyEncoding = Encoding.ASCII,
                IsBodyHtml = false
            };

            foreach (var to in email.Tos)
            {
                mailMessage.To.Add(new MailAddress(to));
            }

            if (email.CCs != null)
            {
                foreach (var cc in email.CCs)
                {
                    mailMessage.CC.Add(new MailAddress(cc));
                }
            }

            if (email.BCCs != null)
            {
                foreach (var bcc in email.BCCs)
                {
                    mailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }

            if (email.Attachments != null)
            {
                foreach (var filePath in email.Attachments)
                {
                    mailMessage.Attachments.Add(new Attachment(filePath));
                }
            }

            // send mail
            smtpClient.Send(mailMessage);
        }

        private static string JoinBodyAndFooterText(string body, string footerText)
        {
            if (footerText == null) return body;
            return body + Environment.NewLine + Environment.NewLine + footerText;
        }
    }
}
