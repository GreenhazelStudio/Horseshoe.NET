using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;

namespace Horseshoe.NET.Text
{
    public static class Extensions
    {
        public static string Clean(this string text, TextCleanMode textCleanMode = TextCleanMode.None, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            return TextClean.CleanString(text, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);
        }

        public static string MultilineTrim(this string text)
        {
            return TextUtil.MultilineTrim(text);
        }

        public static string PadCenter(this string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            return TextUtil.PadCenter(text, totalWidth, padPolicy: padPolicy, padChar: padChar, truncPolicy: truncPolicy, truncMarker: truncMarker);
        }

        public static string PadLeft(this string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            return TextUtil.PadLeft(text, totalWidth, padPolicy: padPolicy, padChar: padChar, truncPolicy: truncPolicy, truncMarker: truncMarker);
        }

        public static string PadRight(this string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            return TextUtil.PadRight(text, totalWidth, padPolicy: padPolicy, padChar: padChar, truncPolicy: truncPolicy, truncMarker: truncMarker);
        }

        public static string Repeat(this string text, int numberOfTimes)
        {
            return TextUtil.Repeat(text, numberOfTimes);
        }

        public static string Trunc(this string text, int targetLength, TruncatePolicy truncPolicy = TruncatePolicy.Simple, string truncMarker = null, PadPolicy padPolicy = default, char? padChar = null)
        {
            return TextUtil.Trunc(text, targetLength, truncPolicy: truncPolicy, truncMarker: truncMarker, padPolicy: padPolicy, padChar: padChar);
        }

        public static string TruncLeft(this string text, int targetLength, TruncatePolicy truncPolicy = TruncatePolicy.Simple, string truncMarker = null, PadPolicy padPolicy = default, char? padChar = null)
        {
            return TextUtil.TruncLeft(text, targetLength, truncPolicy: truncPolicy, truncMarker: truncMarker, padPolicy: padPolicy, padChar: padChar);
        }

        public static string Zap(this string text, string defaultValue = null, TextCleanMode textCleanMode = TextCleanMode.None, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            return TextUtil.Zap(text, defaultValue: defaultValue, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);
        }

        public static int? ZapNInt(this string text, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return TextUtil.ZapNInt(text, numberStyles: numberStyles, provider: provider);
        }

        public static int ZapInt(this string text, int defaultValue = default, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return TextUtil.ZapInt(text, defaultValue: defaultValue, numberStyles: numberStyles, provider: provider);
        }

        public static StringBuilder AppendIf(this StringBuilder sb, bool criteria, string valueIfTrue, string valueIfFalse = null)
        {
            if (criteria)
            {
                return sb.Append(valueIfTrue);
            }
            else if(valueIfFalse != null)
            {
                return sb.Append(valueIfFalse);
            }
            return sb;
        }

        public static StringBuilder AppendIf(this StringBuilder sb, bool criteria, object valueIfTrue, object valueIfFalse = null)
        {
            if (criteria)
            {
                return sb.Append(valueIfTrue);
            }
            else if (valueIfFalse != null)
            {
                return sb.Append(valueIfFalse);
            }
            return sb;
        }

        public static StringBuilder AppendLineIf(this StringBuilder sb, bool criteria)
        {
            if (criteria)
            {
                return sb.AppendLine();
            }
            return sb;
        }

        public static StringBuilder AppendLineIf(this StringBuilder sb, bool criteria, string valueIfTrue, string valueIfFalse = null)
        {
            if (criteria)
            {
                return sb.AppendLine(valueIfTrue);
            }
            else if (valueIfFalse != null)
            {
                return sb.AppendLine(valueIfFalse);
            }
            return sb;
        }

        public static string ReplaceLast(this string textToSearch, string textToReplace, string replacementText)
        {
            return TextUtil.ReplaceLast(textToSearch, textToReplace, replacementText);
        }

        public static SecureString ToSecureString(this string unsecureString)
        {
            return TextUtil.ConvertToSecureString(unsecureString);
        }

        public static string ToUnsecureString(this SecureString secureString)
        {
            return TextUtil.ConvertToUnsecureString(secureString);
        }

        public static bool IsASCII(this char c)
        {
            return (int)c >= 32 && (int)c <= 126;
        }

        public static bool IsASCII(this string text)
        {
            foreach(char c in text)
            {
                if (!IsASCII(c)) return false;
            }
            return true;
        }

        public static bool ContainsAny(this string text, params string[] contentsToSearchFor)
        {
            return ContainsAny(text, contentsToSearchFor, out _, ignoreCase: false);
        }

        public static bool ContainsAny(this string text, IEnumerable<string> contentsToSearchFor, bool ignoreCase = false)
        {
            return ContainsAny(text, contentsToSearchFor, out _, ignoreCase: ignoreCase);
        }

        public static bool ContainsAny(this string text, IEnumerable<string> contentsToSearchFor, out string contentFound, bool ignoreCase = false)
        {
            if (contentsToSearchFor != null)
            {
                foreach (var content in contentsToSearchFor)
                {
                    if (ignoreCase && text.ToLower().Contains(content.ToLower()))
                    {
                        contentFound = content;
                        return true;
                    }
                    if (!ignoreCase && text.Contains(content))
                    {
                        contentFound = content;
                        return true;
                    }
                }
            }
            contentFound = null; 
            return false;
        }

        public static string ToHtml(this string text)
        {
            var html = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "<br />").Replace("\n", "<br />");
            return html;
        }
    }
}
