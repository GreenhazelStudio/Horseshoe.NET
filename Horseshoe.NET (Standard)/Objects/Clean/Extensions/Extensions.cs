using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects.Clean.Extensions
{
    public static class Extensions
    {
        public static object Zap(this object obj) =>
            Clean.Zap.Object(obj);

        public static string ZapString(this object obj, TextCleanMode textCleanMode = TextCleanMode.RemoveNonprintables, object customTextCleanDictionary = null, char[] charsToRemove = null) =>
            Clean.Zap.String(obj, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove);

        public static bool ZapBool(this object obj, bool defaultValue = false, string[] trueValues = null, string[] falseValues = null, bool ignoreCase = false, bool treatArbitraryAsFalse = false) =>
            Clean.Zap.Bool(obj, defaultValue: defaultValue, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase, treatArbitraryAsFalse: treatArbitraryAsFalse);

        public static bool? ZapNBool(this object obj, string[] trueValues = null, string[] falseValues = null, bool ignoreCase = false, bool treatArbitraryAsFalse = false) =>
            Clean.Zap.NBool(obj, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase, treatArbitraryAsFalse: treatArbitraryAsFalse);

        public static byte ZapByte(this object obj, byte defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Byte(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static byte? ZapNByte(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NByte(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static short ZapShort(this object obj, short defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Short(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static short? ZapNShort(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NShort(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static int ZapInt(this object obj, int defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Int(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static int? ZapNInt(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NInt(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static long ZapLong(this object obj, long defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Long(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static long? ZapNLong(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NLong(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static decimal ZapDecimal(this object obj, decimal defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Decimal(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static decimal? ZapNDecimal(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NDecimal(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static float ZapFloat(this object obj, float defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Float(obj, defaultValue: defaultValue, force: force, numberStyles: numberStyles, provider: provider);

        public static float? ZapNFloat(this object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NFloat(obj, force: force, numberStyles: numberStyles, provider: provider);

        public static double ZapDouble(this object obj, double defaultValue = 0, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.Double(obj, defaultValue: defaultValue, numberStyles: numberStyles, provider: provider);

        public static double? ZapNDouble(this object obj, NumberStyles? numberStyles = null, IFormatProvider provider = null) =>
            Clean.Zap.NDouble(obj, numberStyles: numberStyles, provider: provider);

        public static DateTime ZapDateTime(this object obj, DateTime defaultValue = default, IFormatProvider provider = null, DateTimeStyles? dateTimeStyles = null) =>
            Clean.Zap.DateTime(obj, defaultValue: defaultValue, provider: provider, dateTimeStyles: dateTimeStyles);

        public static DateTime? ZapNDateTime(this object obj, IFormatProvider provider = null, DateTimeStyles? dateTimeStyles = null) =>
            Clean.Zap.NDateTime(obj, provider: provider, dateTimeStyles: dateTimeStyles);

        public static T ZapEnum<T>(this object obj, T defaultValue = default, bool ignoreCase = false, bool suppressErrors = false) where T : struct =>
            Clean.Zap.Enum<T>(obj, defaultValue: defaultValue, ignoreCase: ignoreCase, suppressErrors: suppressErrors);

        public static T? ZapNEnum<T>(this object obj, bool ignoreCase = false, bool suppressErrors = false) where T : struct =>
            Clean.Zap.NEnum<T>(obj, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
    }
}
