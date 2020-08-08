﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Extensions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ObjectClean
{
    public static class Methods
    {
        public static object Zap(object obj)
        {
            if (obj is null || obj is DBNull) return null;
            if (obj is string stringValue) return _ZapString(stringValue, TextCleanMode.RemoveNonprintables, null, null);
            return obj;
        }

        public static string ZapString(object obj, TextCleanMode textCleanMode = TextCleanMode.RemoveNonprintables, object customTextCleanDictionary = null, char[] charsToRemove = null)
        {
            if (obj is null || obj is DBNull) return null;
            return _ZapString(obj.ToString(), textCleanMode, customTextCleanDictionary, charsToRemove);
        }

        static string _ZapString(string stringValue, TextCleanMode textCleanMode, object customTextCleanDictionary, char[] charsToRemove)
        {
            stringValue = stringValue.Trim();
            if (stringValue.Length > 0)
            {
                stringValue = TextClean.CleanString(stringValue, textCleanMode: textCleanMode, customTextCleanDictionary: customTextCleanDictionary, charsToRemove: charsToRemove).Trim();
            }
            if (stringValue.Length == 0) return null;
            return stringValue;
        }

        public static bool ZapBool(object obj, bool defaultValue = false, string[] trueValues = null, string[] falseValues = null, bool ignoreCase = false, bool treatArbitraryAsFalse = false)
        {
            return ZapNBool(obj, trueValues: trueValues, falseValues: falseValues, ignoreCase: ignoreCase, treatArbitraryAsFalse: treatArbitraryAsFalse) ?? defaultValue;
        }

        public static bool? ZapNBool(object obj, string[] trueValues = null, string[] falseValues = null, bool ignoreCase = false, bool treatArbitraryAsFalse = false)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue;
            if (obj is byte byteValue) return byteValue == 1;
            if (obj is short shortValue) return shortValue == 1;
            if (obj is int intValue) return intValue == 1;
            if (obj is long longValue) return longValue == 1L;
            if (obj is decimal decimalValue) return decimalValue == 1M;
            if (obj is float floatValue) return floatValue == 1F;
            if (obj is double doubleValue) return doubleValue == 1D;
            if (obj is string stringValue)
            {
                if (trueValues != null)
                {
                    if (falseValues == null)
                    {
                        throw new UtilityException("'true' values were specified, 'false' values were not.");
                    }
                    if (ignoreCase ? stringValue.InIgnoreCase(trueValues) : stringValue.In(trueValues))
                    {
                        return true;
                    }
                    if ((ignoreCase ? stringValue.InIgnoreCase(falseValues) : stringValue.In(falseValues)) || treatArbitraryAsFalse)
                    {
                        return false;
                    }
                }
                else if (falseValues != null)
                {
                    throw new UtilityException("'false' values were specified, 'true' values were not.");
                }
                if (stringValue.InIgnoreCase("1", "T", "True", "Y", "Yes"))
                {
                    return true;
                }
                if (stringValue.InIgnoreCase("0", "F", "False", "N", "No") || treatArbitraryAsFalse)
                {
                    return false;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to bool");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to bool");
        }

        public static byte ZapByte(object obj, byte defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNByte(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static byte? ZapNByte(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return (byte)(boolValue ? 1 : 0);
            if (obj is byte byteValue) return byteValue;
            var rangeMsg = " is outside the range of " + typeof(byte).FullName;
            if (obj is short shortValue)
            {
                return (shortValue > byte.MaxValue || shortValue < byte.MinValue) && !force
                    ? throw new UtilityException(shortValue + rangeMsg)
                    : (byte)shortValue;
            }
            if (obj is int intValue)
            {
                return (intValue > byte.MaxValue || intValue < byte.MinValue) && !force
                    ? throw new UtilityException(intValue + rangeMsg)
                    : (byte)intValue;
            }
            if (obj is long longValue)
            {
                return (longValue > byte.MaxValue || longValue < byte.MinValue) && !force
                    ? throw new UtilityException(longValue + rangeMsg)
                    : (byte)longValue;
            }
            if (obj is decimal decimalValue)
            {
                return (decimalValue > byte.MaxValue || decimalValue < byte.MinValue) && !force
                    ? throw new UtilityException(decimalValue + rangeMsg)
                    : (byte)decimalValue;
            }
            if (obj is float floatValue)
            {
                return (floatValue > byte.MaxValue || floatValue < byte.MinValue) && !force
                    ? throw new UtilityException(floatValue + rangeMsg)
                    : (byte)floatValue;
            }
            if (obj is double doubleValue)
            {
                return (doubleValue > byte.MaxValue || doubleValue < byte.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (byte)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (byte.TryParse(stringValue, numberStyles ?? default, provider, out byte result))
                    {
                        return result;
                    }
                }
                else if (byte.TryParse(stringValue, out byte result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to byte");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to byte");
        }

        public static short ZapShort(object obj, short defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNShort(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static short? ZapNShort(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return (short)(boolValue ? 1 : 0);
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            var rangeMsg = " is outside the range of " + typeof(short).FullName;
            if (obj is int intValue)
            {
                return (intValue > short.MaxValue || intValue < short.MinValue) && !force
                    ? throw new UtilityException(intValue + rangeMsg)
                    : (short)intValue;
            }
            if (obj is long longValue)
            {
                return (longValue > short.MaxValue || longValue < short.MinValue) && !force
                    ? throw new UtilityException(longValue + rangeMsg)
                    : (short)longValue;
            }
            if (obj is decimal decimalValue)
            {
                return (decimalValue > short.MaxValue || decimalValue < short.MinValue) && !force
                    ? throw new UtilityException(decimalValue + rangeMsg)
                    : (short)decimalValue;
            }
            if (obj is float floatValue)
            {
                return (floatValue > short.MaxValue || floatValue < short.MinValue) && !force
                    ? throw new UtilityException(floatValue + rangeMsg)
                    : (short)floatValue;
            }
            if (obj is double doubleValue)
            {
                return (doubleValue > short.MaxValue || doubleValue < short.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (short)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (short.TryParse(stringValue, numberStyles ?? default, provider, out short result))
                    {
                        return result;
                    }
                }
                else if (short.TryParse(stringValue, out short result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to short");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to short");
        }

        public static int ZapInt(object obj, int defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNInt(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static int? ZapNInt(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            var rangeMsg = " is outside the range of " + typeof(int).FullName;
            if (obj is long longValue)
            {
                return (longValue > int.MaxValue || longValue < int.MinValue) && !force
                    ? throw new UtilityException(longValue + rangeMsg)
                    : (int)longValue;
            }
            if (obj is decimal decimalValue)
            {
                return (decimalValue > int.MaxValue || decimalValue < int.MinValue) && !force
                    ? throw new UtilityException(decimalValue + rangeMsg)
                    : (int)decimalValue;
            }
            if (obj is float floatValue)
            {
                return (floatValue > int.MaxValue || floatValue < int.MinValue) && !force
                    ? throw new UtilityException(floatValue + rangeMsg)
                    : (int)floatValue;
            }
            if (obj is double doubleValue)
            {
                return (doubleValue > int.MaxValue || doubleValue < int.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (int)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (int.TryParse(stringValue, numberStyles ?? default, provider, out int result))
                    {
                        return result;
                    }
                }
                else if (int.TryParse(stringValue, out int result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to int");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to int");
        }

        public static long ZapLong(object obj, long defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNLong(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static long? ZapNLong(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            var rangeMsg = " is outside the range of " + typeof(long).FullName;
            if (obj is decimal decimalValue)
            {
                return (decimalValue > long.MaxValue || decimalValue < long.MinValue) && !force
                    ? throw new UtilityException(decimalValue + rangeMsg)
                    : (long)decimalValue;
            }
            if (obj is float floatValue)
            {
                return (floatValue > long.MaxValue || floatValue < long.MinValue) && !force
                    ? throw new UtilityException(floatValue + rangeMsg)
                    : (long)floatValue;
            }
            if (obj is double doubleValue)
            {
                return (doubleValue > long.MaxValue || doubleValue < long.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (long)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (long.TryParse(stringValue, numberStyles ?? default, provider, out long result))
                    {
                        return result;
                    }
                }
                else if (long.TryParse(stringValue, out long result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to long");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to long");
        }

        public static decimal ZapDecimal(object obj, decimal defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNDecimal(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static decimal? ZapNDecimal(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return decimalValue;
            var rangeMsg = " is outside the range of " + typeof(decimal).FullName;
            if (obj is float floatValue)
            {
                return (floatValue > (float)decimal.MaxValue || floatValue < (float)decimal.MinValue) && !force
                    ? throw new UtilityException(floatValue + rangeMsg)
                    : (decimal)floatValue;
            }
            if (obj is double doubleValue)
            {
                return (doubleValue > (double)decimal.MaxValue || doubleValue < (double)decimal.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (decimal)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (decimal.TryParse(stringValue, numberStyles ?? default, provider, out decimal result))
                    {
                        return result;
                    }
                }
                else if (decimal.TryParse(stringValue, out decimal result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to decimal");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to decimal");
        }

        public static float ZapFloat(object obj, float defaultValue = 0, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNFloat(obj, force: force, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static float? ZapNFloat(object obj, bool force = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return (float)decimalValue;
            if (obj is float floatValue) return floatValue;
            var rangeMsg = " is outside the range of " + typeof(float).FullName;
            if (obj is double doubleValue)
            {
                return (doubleValue > float.MaxValue || doubleValue < float.MinValue) && !force
                    ? throw new UtilityException(doubleValue + rangeMsg)
                    : (float)doubleValue;
            }
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (float.TryParse(stringValue, numberStyles ?? default, provider, out float result))
                    {
                        return result;
                    }
                }
                else if (float.TryParse(stringValue, out float result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to float");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to float");
        }

        public static double ZapDouble(object obj, double defaultValue = 0, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return ZapNDouble(obj, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static double? ZapNDouble(object obj, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is bool boolValue) return boolValue ? 1 : 0;
            if (obj is byte byteValue) return byteValue;
            if (obj is short shortValue) return shortValue;
            if (obj is int intValue) return intValue;
            if (obj is long longValue) return longValue;
            if (obj is decimal decimalValue) return (double)decimalValue;
            if (obj is float floatValue) return floatValue;
            if (obj is double doubleValue) return doubleValue;
            if (obj is string stringValue)
            {
                if (numberStyles.HasValue || provider != null)
                {
                    if (double.TryParse(stringValue, numberStyles ?? default, provider, out double result))
                    {
                        return result;
                    }
                }
                else if (double.TryParse(stringValue, out double result))
                {
                    return result;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to double");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to double");
        }

        public static DateTime ZapDateTime(object obj, DateTime defaultValue = default, IFormatProvider provider = null, DateTimeStyles? dateTimeStyles = null)
        {
            return ZapNDateTime(obj, provider: provider, dateTimeStyles: dateTimeStyles) ?? defaultValue;
        }

        public static DateTime? ZapNDateTime(object obj, IFormatProvider provider = null, DateTimeStyles? dateTimeStyles = null)
        {
            if ((obj = Zap(obj)) is null) return null;
            if (obj is DateTime dateTimeValue) return dateTimeValue;
            if (obj is string stringValue)
            {
                if (provider != null || dateTimeStyles.HasValue)
                {
                    if (DateTime.TryParse(stringValue, provider, dateTimeStyles ?? default, out dateTimeValue))
                    {
                        return dateTimeValue;
                    }
                }
                else if (DateTime.TryParse(stringValue, out dateTimeValue))
                {
                    return dateTimeValue;
                }
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to DateTime");
            }
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to DateTime");
        }

        public static T ZapEnum<T>(object obj, T defaultValue = default, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            return ZapNEnum<T>(obj, ignoreCase: ignoreCase, suppressErrors: suppressErrors) ?? defaultValue;
        }

        public static T? ZapNEnum<T>(object obj, bool ignoreCase = false, bool suppressErrors = false) where T : struct
        {
            if ((obj = Zap(obj)) is null) return null;
            var type = typeof(T);
            if (!type.IsEnum) throw new UtilityException("Not an enum type: " + type.FullName);
            if (obj is T tValue) return tValue;
            object tempObj = null;
            try
            {
                if (obj is byte byteValue) tempObj = Enum.ToObject(type, byteValue);
                if (obj is int intValue) tempObj = Enum.ToObject(type, intValue);
                if (obj is long longValue) tempObj = Enum.ToObject(type, longValue);
            }
            catch
            {
                throw new UtilityException("Cannot convert \"" + obj + "\" to " + type.FullName);
            }
            if (tempObj != null) return (T)tempObj;
            if (obj is string stringValue)
            {
                stringValue = TextClean.CleanString(stringValue, textCleanMode: TextCleanMode.RemoveWhitespace);
                if (Enum.TryParse(stringValue, ignoreCase, out T enumValue))
                {
                    return enumValue;
                }
                if (suppressErrors) return null;
                throw new UtilityException("Cannot convert \"" + stringValue + "\" to " + type.FullName);
            }
            if (suppressErrors) return null;
            throw new UtilityException("Cannot convert object of type \"" + obj.GetType().Name + "\" to " + type.FullName);
        }
    }
}
