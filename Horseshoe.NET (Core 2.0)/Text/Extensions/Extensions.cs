using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;

namespace Horseshoe.NET.Text.Extensions
{
    public static class Extensions
    {
        public static string Clean(this string text, TextCleanRules rules)
        {
            return TextClean.CleanString(text, rules: rules);
        }

        public static string Fill(this string text, int targetLength, bool allowOverflow = false)
        {
            return TextUtil.Fill(text, targetLength, allowOverflow: allowOverflow);
        }

        public static string Fit(this string text, int targetLength, Direction direction = Direction.Left, string padding = null, string leftPadding = null, Direction? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: direction, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        public static string FitCenter(this string text, int targetLength, string padding = null, string leftPadding = null, Direction? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: Direction.Center, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        public static string FitLeft(this string text, int targetLength, string padding = null, string leftPadding = null, Direction? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: Direction.Left, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        public static string FitRight(this string text, int targetLength, string padding = null, string leftPadding = null, Direction? truncateDirection = null, string truncateMarker = null)
        {
            return TextUtil.Fit(text, targetLength, direction: Direction.Right, padding: padding, leftPadding: leftPadding, truncateDirection: truncateDirection, truncateMarker: truncateMarker);
        }

        public static bool HasWhitespace(this string text)
        {
            return TextUtil.HasWhitespace(text);
        }

        public static string MultilineTrim(this string text)
        {
            return TextUtil.MultilineTrim(text);
        }

        public static string Pad(this string text, int targetLength, Direction direction = Direction.Right, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: direction, padding: padding, leftPadding: leftPadding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        public static string PadCenter(this string text, int targetLength, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: Direction.Center, padding: padding, leftPadding: leftPadding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        public static string PadLeft(this string text, int targetLength, string padding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: Direction.Left, padding: padding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        public static string PadRight(this string text, int targetLength, string padding = null, bool cannotExceedTargetLength = false)
        {
            return TextUtil.Pad(text, targetLength, direction: Direction.Right, padding: padding, cannotExceedTargetLength: cannotExceedTargetLength);
        }

        public static string Repeat(this string text, int numberOfTimes)
        {
            return TextUtil.Repeat(text, numberOfTimes);
        }

        public static string Crop(this string text, int targetLength, Direction direction = Direction.Right, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: direction, truncateMarker: truncateMarker);
        }

        public static string CropCenter(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: Direction.Center, truncateMarker: truncateMarker);
        }

        public static string CropLeft(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: Direction.Left, truncateMarker: truncateMarker);
        }

        public static string CropRight(this string text, int targetLength, string truncateMarker = null)
        {
            return TextUtil.Crop(text, targetLength, direction: Direction.Right, truncateMarker: truncateMarker);
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
