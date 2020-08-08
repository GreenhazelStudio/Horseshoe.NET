using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections.Extensions;

namespace Horseshoe.NET.Text
{
    public static class TextUtil
    {
        private static Regex TemplatePartPattern { get; } = new Regex(@"{{[^{}]+}}");

        public static string MultilineTrim(string text)
        {
            if (text == null) return null;
            text = text.Trim().Replace("\r\n", "\n");
            var lines = text.Split('\n')
                .Trim()
                .ToArray();
            text = string.Join(Environment.NewLine, lines);
            return text;
        }

        public static bool HasWhitespace(string text)
        {
            if (text == null) return false;
            return text.Any(c => char.IsWhiteSpace(c));
        }

        public static string BuildFromTemplate(string template, object value)
        {
            var rawTemplateParts = TemplatePartPattern.Matches(template)
                .Cast<Match>()
                .Where(m => m.Success)
                .Select(m => m.Value)
                .Distinct()
                .ToList();
            var templateParts = rawTemplateParts
                .Select(rtp => TemplatePart.Parse(rtp))
                .ToList();
            var finalString = template;
            foreach (var templatePart in templateParts)
            {
                finalString = finalString.Replace(templatePart.RawTemplatePart, templatePart.GetReplacementTemplatePart(value));
            }
            return finalString;
        }

        public static string Fill(string text, int targetLength, bool allowOverflow = false, bool rtl = false)
        {
            if (text == null) return null;
            if (targetLength <= 0) return "";
            if (text.Length == 0) text = " ";
            if (text.Length == 1) return new string(text[0], targetLength);
            var sb = new StringBuilder();
            while (sb.Length < targetLength)
            {
                sb.Append(text);
            }
            if (allowOverflow) return sb.ToString();
            return rtl
                ? Crop(sb.ToString(), targetLength, direction: Direction.Left)
                : Crop(sb.ToString(), targetLength);
        }

        public static string Pad(string text, int targetLength, Direction direction = Direction.Right, string padding = null, string leftPadding = null, bool cannotExceedTargetLength = false)
        {
            if (text == null) return null;
            if (text.Length == targetLength) return text;
            if (text.Length > targetLength)
            {
                if (cannotExceedTargetLength) throw new ValidationException("Text length (" + text.Length + ") cannot exceed target length (" + targetLength + ")");
                return text;
            }
            if ((padding ?? "").Length == 0) padding = " ";
            if (padding.Length > targetLength) throw new ValidationException("Padding length (" + padding.Length + ") cannot exceed target length (" + targetLength + ")");
            var rtl = leftPadding != null;
            if (leftPadding == null) leftPadding = padding;
            else if (leftPadding.Length == 0) leftPadding = " ";

            switch (direction)
            {
                case Direction.Left:
                    return Fill(leftPadding, targetLength - text.Length) + text;
                case Direction.Center:
                    var sb = new StringBuilder();
                    int temp = (targetLength - text.Length) / 2;  // in case of uneven padding to left and right of text always prefer a smaller left
                    sb.Append(Fill(leftPadding, temp, rtl: rtl));
                    sb.Append(text);
                    temp = targetLength - text.Length - temp;
                    sb.Append(Fill(padding, temp));
                    return sb.ToString();
                case Direction.Right:
                default:
                    return text + Fill(padding, targetLength - text.Length);
            }
        }

        public static string Crop(string text, int targetLength, Direction direction = Direction.Right, string truncateMarker = null)
        {
            truncateMarker = truncateMarker ?? TruncateMarker.None;
            if (text == null) return null;
            if (targetLength <= 0) return "";
            if (text.Length <= targetLength) return text;
            if (truncateMarker.Length > targetLength) throw new ValidationException("Truncate marker length (" + truncateMarker.Length + ") cannot exceed target length (" + targetLength + ")");
            if (truncateMarker.Length == targetLength) return truncateMarker;

            switch (direction)
            {
                case Direction.Left:
                    return truncateMarker + text.Substring(text.Length - targetLength + truncateMarker.Length);
                case Direction.Center:
                    var sb = new StringBuilder();
                    int temp = (targetLength - truncateMarker.Length) / 2;  // in case of uneven characters to left and right of marker always prefer a smaller left
                    if (temp == 0) temp = 1;                        // except when left is 0 and right is 1 in which case switch
                    sb.Append(text.Substring(0, temp));
                    sb.Append(truncateMarker);
                    temp = targetLength - temp - truncateMarker.Length;
                    sb.Append(text.Substring(text.Length - temp));
                    return sb.ToString();
                case Direction.Right:
                default:
                    return text.Substring(0, targetLength - truncateMarker.Length) + truncateMarker;
            }
        }

        public static string Fit(string text, int targetLength, Direction direction = Direction.Left, string padding = null, string leftPadding = null, Direction? truncateDirection = null, string truncateMarker = null)
        {
            if (text == null) return null;
            if (text.Length == targetLength) return text;
            if (text.Length < targetLength) return Pad(text, targetLength, direction: FitSwitchPadDirection(direction), padding: padding, leftPadding: leftPadding);
            return Crop(text, targetLength, direction: truncateDirection ?? FitSwitchTruncateDirection(direction), truncateMarker: truncateMarker);
        }

        static Direction FitSwitchPadDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Center:
                    return Direction.Center;
                case Direction.Right:
                default:
                    return Direction.Left;
            }
        }

        static Direction FitSwitchTruncateDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                case Direction.Right:
                default:
                    return Direction.Right;
                case Direction.Center:
                    return Direction.Center;
            }
        }

        public static string Repeat(string text, int numberOfTimes)
        {
            if (text == null) return null;
            var sb = new StringBuilder();
            for (int i = 0; i < numberOfTimes; i++)
            {
                sb.Append(text);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Reveals the character codes in a block of text.
        /// </summary>
        /// <param name="input">A block of text to process</param>
        /// <returns>A new string with corresponding codes attached to each character</returns>
        public static string ReplaceLast(string input, string textToReplace, string replacementText, bool ignoreCase = false)
        {
            if (input == null) return null;
            if (textToReplace == null || replacementText == null) return input;

            int pos = ignoreCase
                ? input.ToLower().LastIndexOf(textToReplace.ToLower())
                : input.LastIndexOf(textToReplace);

            if (pos < 0) return input;

            return pos + textToReplace.Length == input.Length 
                ? input.Substring(0, pos) + replacementText
                : input.Substring(0, pos) + replacementText + input.Substring(pos + textToReplace.Length);
        }

        public static string Zoom(char c)
        {
            switch (c)
            {
                case ' ':
                    return "[space]";
                case '\t':
                    return "[tab]";
                case '\r':
                    return "[CR]";
                case '\n':
                    return "[LF]";
                default:
                    if (char.IsControl(c))
                    {
                        return "[ctrl-" + (int)c + "]";
                    }
                    else
                    {
                        return "'" + c + "'[" + (int)c + "]";
                    }
            }
        }

        public static string Zoom(string text, bool preserveLineBreaks = false)
        {
            switch (text)
            {
                case null:
                    return "[null]";
                case "":
                    return "[blank]";
                default:
                    var list = text.Select(c => Zoom(c));
                    var result = string.Join(" ", list).Replace("[CR] [LF]", "[CR-LF]");
                    if (preserveLineBreaks)
                    {
                        result = result.Replace("[CR-LF] ", "[CR-LF]\r\n").Replace("[LF] ", "[LF]\n");
                    }
                    return result;
            }
        }

        public static string RevealNullOrBlank(object input, string valueIfNull = "[null]", string valueIfBlank = "[blank]", bool autoTrim = false)
        {
            if (input == null) return valueIfNull;
            if (input is string text)
            {
                if (autoTrim) text = text.Trim();
                if (text.Length == 0) return valueIfBlank;
                return text;
            }
            return input.ToString();
        }

        public static string RevealWhiteSpace(object input, string spaceIndicator = "·", bool suppressSpaceIndicator = false, string tabIndicator = "--->", bool suppressTabIndicator = false, string crIndicator = "[CR]", string lfIndicator = "[LF]", string crlfIndicator = "[CR-LF]", bool suppressCrlfIndicator = false, string valueIfNull = "[null]", string valueIfBlank = "[blank]", bool autoTrim = false, bool preserveLineBreaks = false)
        {
            var text = RevealNullOrBlank(input, valueIfNull: valueIfNull, valueIfBlank: valueIfBlank, autoTrim: autoTrim);

            if (!suppressSpaceIndicator)
            {
                text = text.Replace(" ", spaceIndicator);
            }
            if (!suppressTabIndicator)
            {
                text = text.Replace("\t", tabIndicator);
            }
            if (!suppressCrlfIndicator)
            {
                text = text.Replace("\r\n", crlfIndicator).Replace("\n", lfIndicator).Replace("\r", crIndicator);
                if (preserveLineBreaks)
                {
                    text = text.Replace("[CR-LF]", "[CR-LF]\r\n").Replace("[LF]", "[LF]\n").Replace("[CR]", "[CR]\r");
                }
            }
            return text;
        }

        public static string AppendIf(bool condition, string text, string textToAppend)
        {
            return condition
                ? text + textToAppend
                : text;
        }

        public static string SpaceOutTitleCase(string titleCaseText)
        {
            if (titleCaseText == null) return null;
            var sb = new StringBuilder();
            bool charIsLetter;
            bool charIsLowerCase;
            bool charIsUpperCase;
            bool charIsDigit;
            //bool charIsWhitespace;
            //bool charIsSpecial;
            bool lastCharWasLetter = false;
            bool lastCharWasLowerCase = false;
            bool lastCharWasUpperCase = false;
            bool lastCharWasDigit = false;
            //bool lastCharWasWhitespace = false;
            //bool lastCharWasSpecial = false;
            for (int i = 0; i < titleCaseText.Length; i++)
            {
                char c = titleCaseText[i];

                charIsLetter = char.IsLetter(c);
                charIsLowerCase = false;
                charIsUpperCase = false;
                charIsDigit = false;
                //charIsWhitespace = false;
                //charIsSpecial = false;

                if (charIsLetter)
                {
                    charIsLowerCase = char.IsLower(c);
                    charIsUpperCase = char.IsUpper(c);
                }
                else if (char.IsDigit(c))
                {
                    charIsDigit = true;
                }
                //else if (char.IsWhiteSpace(c))
                //{
                //    charIsWhitespace = true;
                //}
                //else
                //{
                //    charIsSpecial = true;
                //}

                if (i > 0)
                {
                    //                         v
                    // case #1 -- MyString > My String
                    //
                    if (lastCharWasLowerCase && charIsUpperCase)
                    {
                        sb.Append(" ");
                    }

                    //                                v
                    // case #2 -- MySOAString > My SOA String
                    //                   012345678910
                    else if (lastCharWasUpperCase && charIsUpperCase && titleCaseText.Length >= i + 2 && char.IsLower(titleCaseText[i + 1]))
                    {
                        sb.Append(" ");
                    }

                    //                            v   v
                    // case #3 -- My123String > My 123 String
                    //
                    else if ((lastCharWasDigit && charIsLetter) || (lastCharWasLetter && charIsDigit))
                    {
                        sb.Append(" ");
                    }
                }

                sb.Append(c);
                lastCharWasLowerCase = charIsLowerCase;
                lastCharWasUpperCase = charIsUpperCase;
                lastCharWasLetter = charIsLetter;
                lastCharWasDigit = charIsDigit;
                //lastCharWasWhitespace = charIsWhitespace;
                //lastCharWasSpecial = charIsSpecial;
            }
            return sb.ToString();
        }

        public static SecureString ConvertToSecureString(string unsecureString)
        {
            if (unsecureString == null) throw new ArgumentNullException(nameof(unsecureString));

            var secureString = new SecureString();
            foreach(char c in unsecureString)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        // https://blogs.msdn.microsoft.com/fpintos/2009/06/12/how-to-properly-convert-securestring-to-string/
        public static string ConvertToUnsecureString(SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException(nameof(secureString));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string Dump(DataTable dataTable)
        {
            var sb = new StringBuilder();
            var colWidths = new int[dataTable.Columns.Count];
            int _width;

            // prep widths - column names
            for (int i = 0; i < colWidths.Length; i++)
            {
                colWidths[i] = dataTable.Columns[i].ColumnName.Length;
            }

            // prep widths - data values
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < colWidths.Length; i++)
                {
                    _width = _DumpDatum(row[i]).Length;
                    if (_width > colWidths[i])
                    {
                        colWidths[i] = _width;
                    }
                }
            }

            // build column headers
            for (int i = 0; i < colWidths.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(dataTable.Columns[i].ColumnName.PadRight(colWidths[i]));
            }
            sb.AppendLine();

            // build separators
            for (int i = 0; i < colWidths.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(" ");
                }
                sb.Append("".PadRight(colWidths[i], '-'));
            }
            sb.AppendLine();

            // build data rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(_DumpDatum(row[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static string Dump(IEnumerable<object[]> objectArrays, string[] columnNames = null)
        {
            var sb = new StringBuilder();
            var colWidths = new int[objectArrays.Max(a => a?.Length ?? 0)];
            int _width;

            if (columnNames != null)
            {
                if (columnNames.Length > colWidths.Length)
                {
                    throw new UtilityException("The supplied columns exceed the width of the data: " + columnNames.Length + " / " + colWidths.Length);
                }

                // prep widths - column names
                for (int i = 0; i < columnNames.Length; i++)
                {
                    colWidths[i] = columnNames[i].Length;
                }
            }

            // prep widths - data values
            foreach (var array in objectArrays)
            {
                if (array == null)
                {
                    continue;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    _width = _DumpDatum(array[i]).Length;
                    if (_width > colWidths[i])
                    {
                        colWidths[i] = _width;
                    }
                }
            }

            if (columnNames != null)
            {
                // build column headers
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(columnNames[i].PadRight(colWidths[i]));
                }
                sb.AppendLine();

                // build separators
                for (int i = 0; i < colWidths.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append("".PadRight(colWidths[i], '-'));
                }
                sb.AppendLine();
            }

            // build data rows
            foreach (var array in objectArrays)
            {
                if (array == null)
                {
                    sb.AppendLine();
                    continue;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(_DumpDatum(array[i]).PadRight(colWidths[i]));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static string _DumpDatum(object o)
        {
            if (o == null || o is DBNull) return "[null]";
            if (o is string str) return "\"" + str + "\"";
            return o.ToString();
        }
    }
}
