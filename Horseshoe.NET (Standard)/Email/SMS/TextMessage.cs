using System;

namespace Horseshoe.NET.Email.SMS
{
    public static class TextMessage
    {
        public static event TextMessageSent Sent;

        public static void Send(SMSInfo info, SmtpConnectionInfo connectionInfo = null)
        {
            Validate(info);
            var mobileNumber = SMSUtil.ValidateMobileNumber(info.MobileNumber);
            var recipientAddress = SMSUtil.BuildTextRecipientAddress(mobileNumber, info.Carrier.Value);
            PlainEmail.Send
            (
                new EmailInfo
                {
                    From = info?.From ?? Settings.DefaultFrom,
                    To = recipientAddress,
                    Subject = info.Subject,
                    Body = info.Message,
                },
                connectionInfo: connectionInfo
            );
            Sent?.Invoke(recipientAddress, info.Message);
        }

        private static void Validate(SMSInfo info)
        {
            if (info == null) throw new UtilityException("info cannot be null");
            if (info.MobileNumber == null) throw new UtilityException("MobileNumber cannot be null");
            if (!info.Carrier.HasValue) throw new UtilityException("Carrier cannot be null");
        }
    }
}
