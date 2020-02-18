﻿using System;

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
            string subject = null,
            string from = null,
            SmtpConnectionInfo connectionInfo = null
        )
        {
            if (mobileNumber == null) throw new ValidationException("MobileNumber cannot be null");
            if (!carrier.HasValue) throw new ValidationException("Carrier cannot be null");
            mobileNumber = SmsUtil.ValidateMobileNumber(mobileNumber);

            var recipientAddress = SmsUtil.BuildTextRecipientAddress(mobileNumber, carrier.Value);

            PlainEmail.Send
            (
                message,
                subject,
                to: recipientAddress,
                from: from ?? SmsSettings.DefaultFrom,
                connectionInfo: connectionInfo
            );
            Sent?.Invoke(recipientAddress, message);
        }
    }
}