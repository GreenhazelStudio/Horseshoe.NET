using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static bool IsConfigurationServiceLoaded() => Configuration != null;

        public static bool Has(string key, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            return Get(key, required: false, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded) != null;
        }

        public static string Get(string key, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            if (Configuration == null)
            {
                if (suppressErrorIfConfigurationServiceNotLoaded) return null;
                throw new ConfigurationException("Configuration service not loaded: see Config.LoadConfigurationService()");
            }
            var value = Configuration[key];
            if (value == null && required)
            {
                throw new ConfigurationException("Required configuration not found: " + key);
            }
            return value;
        }

        public static T Get<T>(string key, Func<string, T> parseFunc = null, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false) where T : class
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
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

        public static byte GetByte(string key, byte defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            return GetNByte(key, required: required, numberStyles: numberStyles, provider: provider, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded) ?? defaultValue;
        }

        public static byte? GetNByte(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            if (Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded) is string stringValue)   // bypass 'required' error
            {
                if (stringValue.EndsWith("[hex]"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 5);
                    numberStyles = numberStyles ?? NumberStyles.HexNumber;
                }
                return Zap.NByte(stringValue, numberStyles: numberStyles, provider: provider);
            }
            return null;
        }

        public static byte[] GetBytes(string key, bool required = false, Encoding encoding = default, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
            if (value == null) return null;
            return encoding.GetBytes(value);
        }

        public static int GetInt(string key, int defaultValue = default, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            return GetNInt(key, required: required, numberStyles: numberStyles, provider: provider, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded) ?? defaultValue;
        }

        public static int? GetNInt(string key, bool required = false, NumberStyles? numberStyles = null, IFormatProvider provider = null, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            if (Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded) is string stringValue)   // bypass 'required' error
            {
                if (stringValue.EndsWith("[hex]"))
                {
                    stringValue = stringValue.Substring(0, stringValue.Length - 5);
                    numberStyles = numberStyles ?? NumberStyles.HexNumber;
                }
                return Zap.NInt(stringValue, numberStyles: numberStyles, provider: provider);
            }
            return null;
        }

        public static bool GetBool(string key, bool defaultValue = false, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
            return Zap.Bool(value, defaultValue: defaultValue);
        }

        public static bool? GetNBool(string key, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
            return Zap.NBool(value);
        }

        public static T GetEnum<T>(string key, T defaultValue = default, bool ignoreCase = false, bool required = false, bool suppressErrors = false, bool suppressErrorIfConfigurationServiceNotLoaded = false) where T : struct
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
            return Zap.Enum<T>(value, defaultValue: defaultValue, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static T? GetNEnum<T>(string key, bool ignoreCase = false, bool required = false, bool suppressErrors = false, bool suppressErrorIfConfigurationServiceNotLoaded = false) where T : struct
        {
            var value = Get(key, required: required, suppressErrorIfConfigurationServiceNotLoaded: suppressErrorIfConfigurationServiceNotLoaded);
            return Zap.NEnum<T>(value, ignoreCase: ignoreCase, suppressErrors: suppressErrors);
        }

        public static T[] GetArray<T>(string key, Func<T, bool> filter = null, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            if (Configuration == null)
            {
                if (suppressErrorIfConfigurationServiceNotLoaded) return null;
                throw new ConfigurationException("Configuration service not loaded: see Config.LoadConfigurationService()");
            }
            var section = Configuration.GetSection(key);
            if (!section.Exists())
            {
                if (required)
                {
                    var foundMissingSection = false;
                    var sb = new StringBuilder();
                    var parts = key.Split(':');
                    foreach (var part in parts)
                    {
                        if (sb.Length > 0) sb.Append(":");
                        if (foundMissingSection || Configuration.GetSection(sb + part).Exists())
                        {
                            sb.Append(part);
                        }
                        else
                        {
                            sb.Append("[").Append(part).Append("]");
                            foundMissingSection = true;
                        }
                    }
                    throw new ConfigurationException("Required configuration section not found: " + sb);
                }
                return null;
            }
            var collection = section.Get<IEnumerable<T>>();
            if (filter != null)
            {
                collection = collection.Where(filter);
            }
            return collection.ToArray();
        }

        public static string GetConnectionString(string name, bool required = false, bool suppressErrorIfConfigurationServiceNotLoaded = false)
        {
            if (Configuration == null)
            {
                if (suppressErrorIfConfigurationServiceNotLoaded) return null;
                throw new ConfigurationException("Configuration service not loaded: see Config.LoadConfigurationService()");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid connection string name: " + (name == null ? "[null]" : "[blank]"));
            }
            var connectionString = Configuration.GetConnectionString(name);
            if (connectionString == null && required)
            {
                throw new ConfigurationException("Connection string not found: " + name);
            }
            return connectionString;
        }
    }
}