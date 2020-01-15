using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Horseshoe.NET.IO.WebServices
{
    public static class WebServiceUtil
    {
        public static Action<string> RawResponseReceived;

        public static Action<string> JsonPayloadGenerated;

        private static Cryptography.CryptoOptions CryptoOptions { get; } = new Cryptography.CryptoOptions
        {
            Algorithm = new RijndaelManaged(),
            KeyText = "l)7Be!2X6y&ujB-%"
        };

        public static string Encrypt(string plainText)
        {
            var cipherText = Cryptography.Encrypt.String(plainText, CryptoOptions);
            return cipherText;
        }

        public static string Decrypt(string cipherText)
        {
            var plainText = Cryptography.Decrypt.String(cipherText, CryptoOptions);
            return plainText;
        }

        public static SecureString DecryptSecure(string cipherText)
        {
            var secureString = Cryptography.Decrypt.SecureString(cipherText, CryptoOptions);
            return secureString;
        }

        private static Regex PropertyNameFromBackingFieldRegex { get; } = new Regex(@"(?<=\<)[A-Z_]+(?=\>k__BackingField)", RegexOptions.IgnoreCase);
        private static Regex BackingFieldRegex { get; } = new Regex(@"\<[A-Z_]+\>k__BackingField", RegexOptions.IgnoreCase);

        public static string ZapBackingFields(string rawSerializedText)
        {
            if (rawSerializedText != null)
            {
                var backingFields = new List<string>();

                foreach (Match match in BackingFieldRegex.Matches(rawSerializedText))
                {
                    backingFields.Add(match.Value);
                }

                if (backingFields.Any())
                {
                    backingFields = backingFields.Distinct().ToList();

                    foreach (var backingField in backingFields)
                    {
                        var propertyName = PropertyNameFromBackingFieldRegex.Match(backingField).Value;
                        rawSerializedText = rawSerializedText.Replace(backingField, propertyName);
                    }
                }
            }
            return rawSerializedText;
        }
    }
}
