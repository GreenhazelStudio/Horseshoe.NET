﻿using System;
using System.Configuration;
using System.Globalization;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Application
{
    public static class Config
    {
        public static bool Has(string key, bool required = false)
        {
            return Get(key, required) != null;
        }

        public static string Get(string key, bool required = false)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null && required)
            {
                throw new ConfigurationException("Required configuration not found: " + key);
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
            if (Get(key, required: required) is string stringValue)
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
            if (Get(key, required: required) is string stringValue)
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

        public static T GetSection<T>(string path, bool required = false) where T : ConfigurationSection
        {
            var collection = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).GetSection(path) as T;
            if (collection == null)
            {
                if (required)
                {
                    throw new ConfigurationException("Required configuration section not found: " + path);
                }
                return null;
            }
            return collection;
        }

        public static string GetConnectionString(string name, bool required = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ConfigurationException("Invalid connection string name: " + (name == null ? "[null]" : "[blank]"));
            }
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString == null)
            {
                if (required)
                {
                    throw new ConfigurationException("Connection string not found: " + name);
                }
                return null;
            }
            return connectionString.ConnectionString;
        }
    }
}