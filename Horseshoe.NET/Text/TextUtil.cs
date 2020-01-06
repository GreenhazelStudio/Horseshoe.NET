using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Text
{
    public static class TextUtil
    {
        private static Regex TemplatePartPattern { get; } = new Regex(@"{{[^{}]+}}");

        private static string[] TrueValues { get; } = new[] { "1", "Y", "YES", "T", "TRUE" };

        private static string[] FalseValues { get; } = new[] { "0", "N", "NO", "F", "FALSE" };

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

        /// <summary>
        /// Trims text converting blank text to <c>null</c>
        /// </summary>
        /// <returns></returns>
        public static string Zap(string text, string defaultValue = null, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            if (text == null) return defaultValue;
            text = text.Trim();
            if (text.Any())
            {
                if (textCleanMode != default || customTextCleanDictionary != null || charsToRemove != null)
                {
                    text = TextClean.CleanString(text, textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);
                }
                if (!text.Any()) return defaultValue;
                return text;
            }
            return defaultValue;
        }

        /// <summary>
        /// Converts an object to a string. Returns <c>null</c> if <c>obj</c> is <c>null</c>, <c>DBNull</c> or evalutes to blank text.
        /// </summary>
        /// <returns></returns>
        public static string Zap(object obj, string defaultValue = null, TextCleanMode textCleanMode = default, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            var zapped = ObjectUtil.Zap(obj);
            if (zapped == null) return defaultValue;
            return Zap(zapped.ToString(), defaultValue: defaultValue, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);
        }

        public static byte ZapByte(string text, byte defaultValue = default, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNByte(text, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static byte? ZapNByte(string text, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (numberStyles.HasValue)
            {
                if (byte.TryParse(zapped, numberStyles.Value, provider, out byte byteValue))
                {
                    return byteValue;
                }
            }
            else if (byte.TryParse(zapped, out byte byteValue))
            {
                return byteValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to byte");
        }

        public static byte ZapByte_Hex(string text, byte defaultValue = default)
        {
            return ZapNByte_Hex(text) ?? defaultValue;
        }

        public static byte? ZapNByte_Hex(string text)
        {
            return ZapNByte(text, numberStyles: NumberStyles.HexNumber, provider: null);
        }

        public static short ZapShort(string text, short defaultValue = default, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNShort(text, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static short? ZapNShort(string text, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (numberStyles.HasValue)
            {
                if (short.TryParse(zapped, numberStyles.Value, provider, out short shortValue))
                {
                    return shortValue;
                }
            }
            else if (short.TryParse(zapped, out short shortValue))
            {
                return shortValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to short");
        }

        public static short ZapShort_Hex(string text, short defaultValue = default)
        {
            return ZapNShort_Hex(text) ?? defaultValue;
        }

        public static short? ZapNShort_Hex(string text)
        {
            return ZapNShort(text, numberStyles: NumberStyles.HexNumber, provider: null);
        }

        public static int ZapInt(string text, int defaultValue = default, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNInt(text, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static int? ZapNInt(string text, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (numberStyles.HasValue)
            {
                if (int.TryParse(zapped, numberStyles.Value, provider, out int intValue))
                {
                    return intValue;
                }
            }
            else if (int.TryParse(zapped, out int intValue))
            {
                return intValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to int");
        }

        public static int ZapInt_Hex(string text, int defaultValue = default)
        {
            return ZapNInt_Hex(text) ?? defaultValue;
        }

        public static int? ZapNInt_Hex(string text)
        {
            return ZapNInt(text, numberStyles: NumberStyles.HexNumber, provider: null);
        }

        public static long ZapLong(string text, long defaultValue = default)
        {
            return ZapNLong(text) ?? defaultValue;
        }

        public static long? ZapNLong(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (long.TryParse(zapped, out long longValue))
            {
                return longValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to long");
        }

        public static float ZapFloat(string text, float defaultValue = default)
        {
            return ZapNFloat(text) ?? defaultValue;
        }

        public static float? ZapNFloat(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (float.TryParse(zapped, out float floatValue))
            {
                return floatValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to float");
        }

        public static double ZapDouble(string text, double defaultValue = default)
        {
            return ZapNDouble(text) ?? defaultValue;
        }

        public static double? ZapNDouble(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (double.TryParse(zapped, out double doubleValue))
            {
                return doubleValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to double");
        }

        public static decimal ZapDecimal(string text, decimal defaultValue = default)
        {
            return ZapNDecimal(text) ?? defaultValue;
        }

        public static decimal? ZapNDecimal(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (decimal.TryParse(zapped, out decimal decimalValue))
            {
                return decimalValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to decimal");
        }

        public static bool ZapBoolean(string text, bool defaultValue = default)
        {
            return ZapNBoolean(text) ?? defaultValue;
        }

        public static bool? ZapNBoolean(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (TrueValues.Any(tru => tru.Equals(zapped, StringComparison.OrdinalIgnoreCase))) return true;
            if (FalseValues.Any(fls => fls.Equals(zapped, StringComparison.OrdinalIgnoreCase))) return false;
            throw new UtilityException("\"" + text + "\" cannot be converted to Boolean");
        }

        public static DateTime ZapDateTime(string text, DateTime defaultValue = default)
        {
            return ZapNDateTime(text) ?? defaultValue;
        }

        public static DateTime? ZapNDateTime(string text)
        {
            var zapped = Zap(text);
            if (zapped == null) return null;
            if (DateTime.TryParse(zapped, out DateTime dateTimeValue))
            {
                return dateTimeValue;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to DateTime");
        }

        public static T ZapEnum<T>(string text, T defaultValue = default, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            return ZapNEnum<T>(text, ignoreCase: ignoreCase, suppressErrors: suppressErrors) ?? defaultValue;
        }

        public static T? ZapNEnum<T>(string text, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            if (!typeof(T).IsEnum) throw new UtilityException("Not an enum type: " + typeof(T).FullName);
            string raw = Zap(text, textCleanMode: TextCleanMode.RemoveWhitespace);
            if (raw == null) return null;
            if (Enum.TryParse(raw, ignoreCase, out T enumValue))
            {
                return enumValue;
            }
            if (suppressErrors)
            {
                return null;
            }
            throw new UtilityException("\"" + text + "\" cannot be converted to " + typeof(T).FullName);
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

        public static string PadLeft(string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            if (text == null) return null;
            if (totalWidth < 0) throw new Exception(nameof(totalWidth) + " must be >= 0");
            if (text.Length < totalWidth)
            {
                char c = GetPadChar(padPolicy, padChar);
                return text.PadLeft(totalWidth, c);
            }
            else if(text.Length == totalWidth)
            {
                return text;
            }
            else
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                        return text;
                    case TruncatePolicy.Exception:
                        throw new UtilityException("The text exceeds the target length: " + totalWidth);
                    default:
                        return TruncLeft(text, totalWidth, truncPolicy: truncPolicy, truncMarker: truncMarker);
                }
            }
        }

        public static string PadRight(string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            if (text == null) return null;
            if (totalWidth < 0) throw new Exception(nameof(totalWidth) + " must be >= 0");
            if (text.Length < totalWidth)
            {
                char c = GetPadChar(padPolicy, padChar);
                return text.PadRight(totalWidth, c);
            }
            else if (text.Length == totalWidth)
            {
                return text;
            }
            else
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                        return text;
                    case TruncatePolicy.Exception:
                        throw new UtilityException("The text exceeds the target length: " + totalWidth);
                    default:
                        return Trunc(text, totalWidth, truncPolicy: truncPolicy, truncMarker: truncMarker);
                }
            }
        }

        public static string PadCenter(string text, int totalWidth, PadPolicy padPolicy = PadPolicy.Spaces, char? padChar = null, TruncatePolicy truncPolicy = default, string truncMarker = null)
        {
            if (text == null) return null;
            if (totalWidth < 0) throw new Exception(nameof(totalWidth) + " must be >= 0");
            if (text.Length < totalWidth)
            {
                char c = GetPadChar(padPolicy, padChar);
                int totalPad = totalWidth - text.Length;
                int leftPad = (int)Math.Floor(totalPad / 2.0);   // this technique favors attaching odd padding char to right side rather than left
                return text.PadLeft(leftPad + text.Length, c).PadRight(totalWidth, c);
            }
            else if(text.Length == totalWidth)
            {
                return text;
            }
            else
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                        return text;
                    case TruncatePolicy.Exception:
                        throw new UtilityException("The text exceeds the target length: " + totalWidth);
                    default:
                        return Trunc(text, totalWidth, truncPolicy: truncPolicy, truncMarker: truncMarker);
                }
            }
        }

        static char GetPadChar(PadPolicy padPolicy, char? padChar)
        {
            switch (padPolicy)
            {
                case PadPolicy.Spaces:
                    return ' ';
                case PadPolicy.Zeros:
                    return '0';
                case PadPolicy.Custom:
                    return padChar ?? throw new UtilityException(nameof(padChar) + " was not specified for PadPolicy.Custom");
                default:
                    throw new UtilityException("Pad policy does not apply: '" + padPolicy + "'");
            }
        }

        public static string Trunc(string text, int targetLength, TruncatePolicy truncPolicy = TruncatePolicy.Simple, string truncMarker = null, PadPolicy padPolicy = default, char? padChar = null)
        {
            if (text == null) return null;
            if (targetLength <= 0) return "";
            if (text.Length > targetLength)
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.Simple:
                        return text.Substring(0, targetLength);
                    case TruncatePolicy.Ellipsis:
                        return text.Substring(0, targetLength - 1) + "…";
                    case TruncatePolicy.LongEllipsis:
                        if (targetLength < 3) throw new UtilityException(nameof(targetLength) + " must be >= 3 to use [...] as a truncation marker");
                        return text.Substring(0, targetLength - 3) + "...";
                    case TruncatePolicy.CustomMarker:
                        if (truncMarker == null) throw new UtilityException(nameof(truncMarker) + ", required by TruncatePolicy.CustomMarker, was not specified");
                        if (targetLength < truncMarker.Length) throw new UtilityException(nameof(targetLength) + " must be >= " + truncMarker.Length + " to use [" + truncMarker + "] as a truncation marker: " + targetLength);
                        return text.Substring(0, targetLength - truncMarker.Length) + truncMarker;
                    default:
                        throw new UtilityException("Truncate policy does not apply: '" + truncPolicy + "'");
                }
            }
            else if (text.Length == targetLength)
            {
                return text;
            }
            else
            {
                switch (padPolicy)
                {
                    case PadPolicy.None:
                        return text;
                    default:
                        char c = GetPadChar(padPolicy, padChar);
                        return text.PadRight(targetLength, c);
                }
            }
        }

        public static string TruncLeft(string text, int targetLength, TruncatePolicy truncPolicy = TruncatePolicy.Simple, string truncMarker = null, PadPolicy padPolicy = default, char? padChar = null)
        {
            if (text == null) return null;
            if (targetLength <= 0) return "";
            if (text.Length > targetLength)
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.Simple:
                        return text.Substring(text.Length - targetLength);
                    case TruncatePolicy.Ellipsis:
                        return "…" + text.Substring(text.Length - targetLength + 1);
                    case TruncatePolicy.LongEllipsis:
                        if (targetLength < 3) throw new UtilityException(nameof(targetLength) + " must be >= 3 to use [...] as a truncation marker");
                        return "..." + text.Substring(text.Length - targetLength + 3);
                    case TruncatePolicy.CustomMarker:
                        if (truncMarker == null) throw new UtilityException(nameof(truncMarker) + ", required by TruncatePolicy.CustomMarker, was not specified");
                        if (targetLength < truncMarker.Length) throw new UtilityException(nameof(targetLength) + " must be >= " + truncMarker.Length + " to use [" + truncMarker + "] as a truncation marker: " + targetLength);
                        return truncMarker + text.Substring(text.Length - targetLength + truncMarker.Length);
                    default:
                        throw new UtilityException("Truncate policy does not apply: '" + truncPolicy + "'");
                }
            }
            else if (text.Length == targetLength)
            {
                return text;
            }
            else
            {
                switch (padPolicy)
                {
                    case PadPolicy.None:
                        return text;
                    default:
                        char c = GetPadChar(padPolicy, padChar);
                        return text.PadLeft(targetLength, c);
                }
            }
        }

        public static string Repeat(char c, int numberOfTimes)
        {
            return Repeat(c.ToString(), numberOfTimes);
        }

        public static string Repeat(string str, int numberOfTimes)
        {
            if (str == null) return null;
            var sb = new StringBuilder();
            for (int i = 0; i < numberOfTimes; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }

        public static string Fill(string str, int targetLength, TruncatePolicy truncPolicy = TruncatePolicy.Simple)
        {
            if (str == null) return null;
            var sb = new StringBuilder();
            while (sb.Length + str.Length <= targetLength)
            {
                sb.Append(str);
            }
            if (sb.Length < targetLength)
            {
                switch (truncPolicy)
                {
                    case TruncatePolicy.None:
                        sb.Append(str);
                        break;
                    case TruncatePolicy.Simple:
                        sb.Append(str.Substring(0, targetLength - sb.Length));
                        break;
                    case TruncatePolicy.Ellipsis:
                        sb.Append(str.Substring(0, targetLength - sb.Length - 1) + "…");
                        break;
                    default:
                        throw new UtilityException("Truncate policy does not apply: '" + truncPolicy + "'");
                }
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

        public static string Reveal(object input, string valueIfNull = null, bool nullOrBlank = false, bool crlf = false, bool preserveLineBreaks = false)
        {
            if (input == null)
            {
                return nullOrBlank
                    ? "[null]"
                    : valueIfNull;
            }
            var text = input.ToString();
            if (crlf)
            {
                text = text.Replace("\r\n", "[CR-LF]").Replace("\n", "[LF]").Replace("\r", "[CR]");
                if (preserveLineBreaks)
                {
                    text = text.Replace("[CR-LF]", "[CR-LF]\r\n").Replace("[LF]", "[LF]\n").Replace("[CR]", "[CR]\r");
                }
            }
            if (text.Trim().Length == 0)
            {
                return nullOrBlank
                    ? "[blank]"
                    : text;
            }
            return text;
        }

        public static string AppendIf(string text, string textToAppend, bool condition)
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
