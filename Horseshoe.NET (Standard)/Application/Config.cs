using System;
using System.Globalization;
using System.Text;

using Horseshoe.NET.Objects;
using Horseshoe.NET.Text;

using Microsoft.Extensions.Configuration;

namespace Horseshoe.NET.Application
{
    public static class Config
    {
        private static IConfiguration Configuration { get; set; }

        public static void LoadService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static bool Has(string key)
        {
            return Get(key, required: false) != null;
        }

        public static string Get(string key, bool required = false)
        {
            if (Configuration == null)
            {
                if (required)
                {
                    throw new UtilityException("Configuration service not loaded: see Config.LoadService()");
                }
                return null;
            }
            var value = Configuration[key];
            if (value == null && required)
            {
                throw new UtilityException("Required configuration not found: " + key);
            }
            return value;
        }

        public static T Get<T>(string key, Func<string, T> parseFunc = null, bool required = false) where T : class
        {
            var value = Get(key, required: required);
            if (value == null) return null;
            if (parseFunc != null) return parseFunc.Invoke(value);
            try
            {
                return ObjectUtil.GetInstance<T>(value);
            }
            catch (Exception ex)
            {
                throw new UtilityException("Cannot convert " + value + " to " + typeof(T).FullName, ex);
            }
        }

        public static byte GetByte(string key, byte defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = GetNByte(key, required: required, numberStyles: numberStyles, provider: provider);
            return value ?? defaultValue;
        }

        public static byte? GetNByte(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return TextUtil.ZapNByte(value, numberStyles: numberStyles, provider: provider);
            }
            else
            {
                value = Get(key + "[hex]");
                if (value != null)
                {
                    return TextUtil.ZapNByte_Hex(value);
                }
                else if (required)
                {
                    throw new UtilityException("Required configuration not found: " + key);
                }
                return null;
            }
        }

        public static byte GetByte_Hex(string key, byte defaultValue = default, bool required = false)
        {
            var value = GetNByte_Hex(key, required: required);
            return value ?? defaultValue;
        }

        public static byte? GetNByte_Hex(string key, bool required = false)
        {
            var value = GetNByte(key, required: required, numberStyles: NumberStyles.HexNumber, provider: null);
            return value;
        }

        public static byte[] GetBytes(string key, bool required = false, Encoding encoding = default)
        {
            var value = Get(key, required: required);
            if (value == null) return null;
            return encoding.GetBytes(value);
        }

        public static int GetInt(string key, int defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = GetNInt(key, required: required, numberStyles: numberStyles, provider: provider);
            return value ?? defaultValue;
        }

        public static int? GetNInt(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return TextUtil.ZapNInt(value, numberStyles: numberStyles, provider: provider);
            }
            else
            {
                value = Get(key + "[hex]");
                if (value != null)
                {
                    return TextUtil.ZapNInt_Hex(value);
                }
                else if (required)
                {
                    throw new UtilityException("Required configuration not found: " + key);
                }
                return null;
            }
        }

        public static int GetInt_Hex(string key, int defaultValue = default, bool required = false)
        {
            var value = GetNInt_Hex(key, required: required);
            return value ?? defaultValue;
        }

        public static int? GetNInt_Hex(string key, bool required = false)
        {
            var value = GetNInt(key, required: required, numberStyles: NumberStyles.HexNumber, provider: null);
            return value;
        }

        public static bool GetBoolean(string key, bool defaultValue = false, bool required = false)
        {
            var value = Get(key, required: required);
            return TextUtil.ZapBoolean(value, defaultValue: defaultValue);
        }

        public static bool? GetNBoolean(string key, bool required = false)
        {
            var value = Get(key, required: required);
            return TextUtil.ZapNBoolean(value);
        }

        public static T GetEnum<T>(string key, T defaultValue = default, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(key, required: required);
            return TextUtil.ZapEnum<T>(value, defaultValue: defaultValue, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static T? GetNEnum<T>(string key, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(key, required: required);
            return TextUtil.ZapNEnum<T>(value, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static string GetConnectionString(string name, bool suppressErrors = false)
        {
            if (Configuration == null)
            {
                if (suppressErrors) return null;
                throw new UtilityException("Configuration service not loaded: see Config.LoadService()");
            }
            name = name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                if (suppressErrors) return null;
                throw new UtilityException("Invalid connection string name: " + (name == null ? "[null]" : "[blank]"));
            }
            var connectionString = Configuration.GetConnectionString(name);
            if (connectionString == null)
            {
                if (suppressErrors) return null;
                throw new UtilityException("No connection string has been configured with this name: " + name);
            }
            return connectionString;
        }
    }
}