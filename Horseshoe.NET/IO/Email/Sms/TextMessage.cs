using System;

namespace Horseshoe.NET.IO.Email.SMS
{
    public static class TextMessage
    {
        public static event TextMessageSent Sent;

        public static void Send
        (
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            Send(null, message, mobileNumber: mobileNumber, carrier: carrier, from: from, connectionInfo: connectionInfo);
        }

        public static void Send
        (
            string subject,
            string message,
            string mobileNumber = null,
            Carrier? carrier = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            if (mobileNumber == null) throw new ValidationException("mobileNumber cannot be null");
            if (!carrier.HasValue) throw new ValidationException("carrier cannot be null");
            mobileNumber = SmsUtil.ValidateMobileNumber(mobileNumber);

            var recipientAddress = SmsUtil.BuildTextRecipientAddress(mobileNumber, carrier.Value);

            PlainEmail.Send
            (
                subject,
                message,
                to: recipientAddress,
                from: from ?? SmsSettings.DefaultFrom,
                connectionInfo: connectionInfo
            );
            Sent?.Invoke(recipientAddress, message);
        }
    }
}
