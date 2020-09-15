using System;
using System.Globalization;
using System.Text;

using Horseshoe.NET.Objects;

using Microsoft.Extensions.Configuration;

namespace Horseshoe.NET.Application
{
    public static class Config
    {
        private static IConfiguration Configuration { get; set; }

        public static void LoadConfigurationService(IConfiguration configuration)
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
                throw new UtilityException("Configuration service not loaded: see Config.LoadConfigurationService()");
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
            return GetNByte(key, required: required, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static byte? GetNByte(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return Zap.NByte(value, numberStyles: numberStyles, provider: provider);
            }
            if (!numberStyles.HasValue && provider == null)
            {
                value = Get(key + "[hex]");
                if (value != null)
                {
                    return Zap.NByte(value, numberStyles: NumberStyles.HexNumber);
                }
            }
            if (required)
            {
                throw new UtilityException("Required configuration not found: " + key);
            }
            return null;
        }

        public static byte[] GetBytes(string key, bool required = false, Encoding encoding = default)
        {
            var value = Get(key, required: required);
            if (value == null) return null;
            return encoding.GetBytes(value);
        }

        public static int GetInt(string key, int defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            return GetNInt(key, required: required, numberStyles: numberStyles, provider: provider) ?? defaultValue;
        }

        public static int? GetNInt(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null)
        {
            var value = Get(key);
            if (value != null)
            {
                return Zap.NInt(value, numberStyles: numberStyles, provider: provider);
            }
            if (!numberStyles.HasValue && provider == null)
            {
                value = Get(key + "[hex]");
                if (value != null)
                {
                    return Zap.NInt(value, numberStyles: NumberStyles.HexNumber);
                }
            }
            if (required)
            {
                throw new UtilityException("Required configuration not found: " + key);
            }
            return null;
        }

        public static bool GetBool(string key, bool defaultValue = false, bool required = false)
        {
            var value = Get(key, required: required);
            return Zap.Bool(value, defaultValue: defaultValue);
        }

        public static bool? GetNBool(string key, bool required = false)
        {
            var value = Get(key, required: required);
            return Zap.NBool(value);
        }

        public static T GetEnum<T>(string key, T defaultValue = default, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(key, required: required);
            return Zap.Enum<T>(value, defaultValue: defaultValue, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static T? GetNEnum<T>(string key, bool ignoreCase = false, bool required = false, bool suppressErrors = false) where T : struct
        {
            var value = Get(key, required: required);
            return Zap.NEnum<T>(value, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static string GetConnectionString(string name, bool required = false)
        {
            if (Configuration == null)
            {
                throw new UtilityException("Configuration service not loaded: see Config.LoadConfigurationService()");
            }
            name = name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid connection string name: " + (name == null ? "[null]" : "[blank]"));
            }
            var connectionString = Configuration.GetConnectionString(name);
            if (connectionString == null)
            {
                if (required) 
                {
                    throw new UtilityException("Connection string not found: " + name);
                }
                return null;
            }
            return connectionString;
        }
    }
}