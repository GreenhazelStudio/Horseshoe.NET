using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Horseshoe.NET.Application;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects
{
    public static class ObjectUtil
    {
        public static bool IsNull(object obj)
        {
            return obj is null || obj is DBNull;
        }

        public static string ZapString(object obj, string defaultValue = null)
        {
            if (IsNull(obj)) return defaultValue;
            return obj.ToString();
        }

        public static byte ZapByte(object obj, byte defaultValue = default)
        {
            return ZapNByte(obj) ?? defaultValue;
        }

        public static byte? ZapNByte(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is byte byteValue) return byteValue;
            try
            {
                return Convert.ToByte(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to byte", ex);
            }
        }

        public static short ZapShort(object obj, short defaultValue = default)
        {
            return ZapNShort(obj) ?? defaultValue;
        }

        public static short? ZapNShort(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is short shortValue) return shortValue;
            try
            {
                return Convert.ToInt16(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to short", ex);
            }
        }

        public static int ZapInt(object obj, int defaultValue = default)
        {
            return ZapNInt(obj) ?? defaultValue;
        }

        public static int? ZapNInt(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is int intValue) return intValue;
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to int", ex);
            }
        }

        public static long ZapLong(object obj, long defaultValue = default)
        {
            return ZapNLong(obj) ?? defaultValue;
        }

        public static long? ZapNLong(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is long longValue) return longValue;
            try
            {
                return Convert.ToInt64(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to long", ex);
            }
        }

        public static float ZapFloat(object obj, float defaultValue = default)
        {
            return ZapNFloat(obj) ?? defaultValue;
        }

        public static float? ZapNFloat(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is float floatValue) return floatValue;
            try
            {
                return (float)Convert.ToDouble(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to float", ex);
            }
        }

        public static double ZapDouble(object obj, double defaultValue = default)
        {
            return ZapNDouble(obj) ?? defaultValue;
        }

        public static double? ZapNDouble(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is double doubleValue) return doubleValue;
            try
            {
                return Convert.ToDouble(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to double", ex);
            }
        }

        public static decimal ZapDecimal(object obj, decimal defaultValue = default)
        {
            return ZapNDecimal(obj) ?? defaultValue;
        }

        public static decimal? ZapNDecimal(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is decimal decimalValue) return decimalValue;
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to decimal", ex);
            }
        }

        public static bool ZapBoolean(object obj, bool defaultValue = default)
        {
            return ZapNBoolean(obj) ?? defaultValue;
        }

        public static bool? ZapNBoolean(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is bool boolValue) return boolValue;
            if (obj is string stringValue) return TextUtil.ZapNBoolean(stringValue);
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to bool", ex);
            }
        }

        public static DateTime ZapDateTime(object obj, DateTime defaultValue = default)
        {
            return ZapNDateTime(obj) ?? defaultValue;
        }

        public static DateTime? ZapNDateTime(object obj)
        {
            if (IsNull(obj)) return null;
            if (obj is DateTime dateTimeValue) return dateTimeValue;
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch (Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to DateTime", ex);
            }
        }

        public static T ZapEnum<T>(object obj, T defaultValue = default) where T : struct
        {
            return ZapNEnum<T>(obj) ?? defaultValue;
        }

        public static T? ZapNEnum<T>(object obj, bool ignoreCase = false) where T : struct
        {
            if (!typeof(T).IsEnum) throw new UtilityException("Not an enum type: " + typeof(T).FullName);
            if (IsNull(obj)) return null;
            if (obj is T enumValue) return enumValue;
            if (obj is string stringValue) return TextUtil.ZapNEnum<T>(stringValue, ignoreCase: ignoreCase);
            try
            {
                return (T)Enum.ToObject(typeof(T), obj);
            }
            catch(Exception ex)
            {
                throw new UtilityException("\"" + obj + "\" cannot be converted to " + typeof(T).FullName + " due to: " + ex.Message, ex);
            }
        }

        public static PropertyInfo[] GetProperties<T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where T : class
        {
            return GetProperties(typeof(T), bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            var properties = bindingFlags.HasValue
                ? type.GetProperties(bindingFlags.Value)
                : type.GetProperties();
            if (filter != null)
            {
                properties = properties
                    .Where(filter)
                    .ToArray();
            }
            return properties;
        }

        public static PropertyInfo[] GetPublicStaticProperties<T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where T : class
        {
            return GetPublicStaticProperties(typeof(T), bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetPublicStaticProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Static | (bindingFlags ?? default);
            return GetProperties(type, bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetPublicInstanceProperties<T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where T : class
        {
            return GetPublicInstanceProperties(typeof(T), bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetPublicInstanceProperties(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Instance | (bindingFlags ?? default);
            return GetProperties(type, bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo GetProperty<T>(string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            return GetProperty(typeof(T), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetProperty(object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (obj == null) return null;
            return GetProperty(obj.GetType(), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetProperty(Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (type == null) throw new UtilityException("type cannot be null");
            if (name == null) throw new UtilityException("name cannot be null");
            if (!name.Equals(TextUtil.Zap(name, textCleanMode: TextCleanMode.RemoveWhitespace))) throw new UtilityException("name cannot contain whitespace");
            var searchCriteria = SearchCriteria.Equals(name, ignoreCase: ignoreCase);
            bool filter(PropertyInfo property) => searchCriteria.Evaluate(property.Name);
            var properties = GetProperties(type, bindingFlags: bindingFlags, filter: filter);
            switch (properties.Length)
            {
                case 0:
                    return null;
                case 1:
                    return properties[0];
                default:
                    throw new UtilityException("More than one property matches criteria: name = " + name + (ignoreCase ? " (ignore case)" : ""));
            }
        }

        public static PropertyInfo GetPublicStaticProperty<T>(string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            return GetPublicStaticProperty(typeof(T), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetPublicStaticProperty(Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Static | (bindingFlags ?? default);
            return GetProperty(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetPublicInstanceProperty<T>(string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return GetPublicInstanceProperty(typeof(T), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetPublicInstanceProperty(Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Instance | (bindingFlags ?? default);
            return GetProperty(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue[] GetPropertyValues(object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            if (obj == null) return null;
            var properties = GetProperties(obj.GetType(), bindingFlags: bindingFlags, filter: filter);
            return properties
                .Select(p => new PropertyValue(p, p.GetValue(obj)) as IPropertyValue)
                .ToArray();
        }

        public static IPropertyValue[] GetPublicStaticPropertyValues<T>(BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null) where T : class
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Static | (bindingFlags ?? default);
            var properties = GetProperties<T>(bindingFlags: bindingFlags, filter: filter);
            return properties
                .Select(p => new PropertyValue(p, p.GetValue(null)) as IPropertyValue)
                .ToArray();
        }

        public static IPropertyValue[] GetPublicStaticPropertyValues(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Static | (bindingFlags ?? default);
            var properties = GetProperties(type, bindingFlags: bindingFlags, filter: filter);
            return properties
                .Select(p => new PropertyValue(p, p.GetValue(null)) as IPropertyValue)
                .ToArray();
        }

        public static IPropertyValue<P>[] GetPublicStaticPropertyValuesOfType<P>(Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Static | (bindingFlags ?? default);
            var properties = GetProperties(type, bindingFlags: bindingFlags, filter: filter);
            return properties
                .Where(p => p.PropertyType.IsAssignableFrom(typeof(P)))
                .Select(p => new PropertyValue<P>(p, (P)p.GetValue(null)) as IPropertyValue<P>)
                .ToArray();
        }

        public static IPropertyValue[] GetPublicInstancePropertyValues(object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            bindingFlags = BindingFlags.Public | BindingFlags.Instance | (bindingFlags ?? default);
            return GetPropertyValues(obj, bindingFlags: bindingFlags, filter: filter);
        }

        public static IPropertyValue<P> GetPropertyValue<P>(object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (obj == null) return null;
            var property = GetProperty(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue<P>(property, (P)property.GetValue(obj));
        }

        public static IPropertyValue GetPropertyValue(object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (obj == null) return null;
            var property = GetProperty(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue(property, property.GetValue(obj));
        }

        public static IPropertyValue<P> GetPublicStaticPropertyValue<T, P>(string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            var property = GetPublicStaticProperty<T>(name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue<P>(property, (P)property.GetValue(null));
        }

        public static IPropertyValue<P> GetPublicStaticPropertyValue<P>(Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            var property = GetPublicStaticProperty(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue<P>(property, (P)property.GetValue(null));
        }

        public static IPropertyValue GetPublicStaticPropertyValue<T>(string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            var property = GetPublicStaticProperty<T>(name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue(property, property.GetValue(null));
        }

        public static IPropertyValue GetPublicStaticPropertyValue(Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            var property = GetPublicStaticProperty(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue(property, property.GetValue(null));
        }

        public static IPropertyValue<P> GetPublicInstancePropertyValue<T, P>(T obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            if (obj == null) return null;
            var property = GetPublicInstanceProperty<T>(name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue<P>(property, (P)property.GetValue(obj));
        }

        public static IPropertyValue<P> GetPublicInstancePropertyValue<P>(object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (obj == null) return null;
            var property = GetPublicInstanceProperty(obj.GetType(), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue<P>(property, (P)property.GetValue(obj));
        }

        public static IPropertyValue GetPublicInstancePropertyValue<T>(T obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null) where T : class
        {
            if (obj == null) return null;
            var property = GetPublicInstanceProperty<T>(name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue(property, property.GetValue(obj));
        }

        public static IPropertyValue GetPublicInstancePropertyValue(object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            if (obj == null) return null;
            var property = GetPublicInstanceProperty(obj.GetType(), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
            if (property == null) return null;
            return new PropertyValue(property, property.GetValue(obj));
        }

        public static IPropertyValue<P> GetNestedPropertyValue<P>(object obj, string propertyFullPath, bool ignoreCase = false)
        {
            var propertyNameParts = propertyFullPath.Split('.');
            IPropertyValue<P> propertyValue = null;
            foreach (var propertyName in propertyNameParts)
            {
                propertyValue = GetPropertyValue<P>(obj, propertyName, ignoreCase: ignoreCase);
                obj = propertyValue.Value;
            }
            return propertyValue;
        }

        public static IPropertyValue GetNestedPropertyValue(object obj, string propertyFullPath, bool ignoreCase = false)
        {
            var propertyNameParts = propertyFullPath.Split('.');
            IPropertyValue propertyValue = null;
            foreach (var propertyName in propertyNameParts)
            {
                propertyValue = GetPropertyValue(obj, propertyName, ignoreCase: ignoreCase);
                obj = propertyValue.Value;
            }
            return propertyValue;
        }

        /// <summary>
        /// Copies public read/write properties from one object to another  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="deepCopy"></param>
        public static T EasyMapToNew<T>(object src, bool deepCopy = false, bool ignoreCase = false, bool ignoreUnmappables = false) where T : class
        {
            var dest = (T)GetInstance<T>();
            EasyMap(src, dest, deepCopy: deepCopy, ignoreCase: ignoreCase, ignoreUnmappables: ignoreUnmappables);
            return dest;
        }

        /// <summary>
        /// Copies public read/write properties from one object to another  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="deepCopy"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="ignoreUnmappables"></param>
        public static void EasyMap(object src, object dest, bool deepCopy = false, bool ignoreCase = false, bool ignoreUnmappables = false)
        {
            if (src == null) throw new ValidationException("Argument '" + nameof(src) + "' cannot be null");
            if (dest == null) throw new ValidationException("Argument '" + nameof(dest) + "' cannot be null");

            var srcType = src.GetType();
            if (!srcType.IsClass) throw new ValidationException("Argument '" + nameof(src) + "' must be a reference type, detected non-class: " + srcType.FullName);
            
            var destType = dest.GetType();
            if (!destType.IsClass) throw new ValidationException("Argument '" + nameof(dest) + "' must be a reference type, detected non-class: " + destType.FullName);
            
            var srcProperties = GetPublicInstanceProperties(srcType, filter: (PropertyInfo p) => p.CanRead);
            var srcValues = srcProperties
                .Select(p => p.GetValue(src))
                .ToArray();

            for (int i = 0; i < srcProperties.Length; i++)
            {
                var srcPropertyType = srcProperties[i].PropertyType;
                if (deepCopy && srcPropertyType.IsClass && srcValues[i] != null)
                {
                    if (!srcPropertyType.GetConstructors().Any(c => c.IsPublic && !c.GetParameters().Any()))
                    {
                        if (!ignoreUnmappables) throw new UtilityException("deep copy cannot instantiate property type: " + srcPropertyType.FullName);
                        SetInstanceProperty(dest, srcProperties[i].Name, srcValues[i], ignoreCase: ignoreCase, suppressErrors: ignoreUnmappables);
                        continue;
                    }
                    var newValue = GetInstance(srcPropertyType);
                    EasyMap(srcValues[i], newValue, ignoreUnmappables: ignoreUnmappables);
                    SetInstanceProperty(dest, srcProperties[i].Name, newValue, ignoreCase: ignoreCase, suppressErrors: ignoreUnmappables);
                }
                else
                {
                    SetInstanceProperty(dest, srcProperties[i].Name, srcValues[i], ignoreCase: ignoreCase, suppressErrors: ignoreUnmappables);
                }
            }
        }

        /// <summary>
        /// Finds the runtime type respresented by the fully qualified class name
        /// </summary>
        /// <param name="className"></param>
        /// <param name="suppressErrors"></param>
        /// <returns></returns>
        public static Type GetType(string className, string assemblyName = null, bool suppressErrors = false)
        {
            if (className == null)
            {
                if (suppressErrors) return null;
                throw new ArgumentNullException(nameof(className));
            }
            try
            {
                // step 1 - direct type loading (seldom works)
                Type type = Type.GetType(className);
                if (type != null) return type;

                // step 2 - load type from loaded assemblies (e.g. System.Core, Horseshoe.NET, etc.)
                if (type == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = assembly.GetType(className);
                        if (type != null) return type;
                    }
                }

                // step 3 - load assembly and type
                if (assemblyName != null)
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (assembly != null)
                    {
                        type = assembly.GetType(className);
                        if (type != null) return type;
                    }
                }

                throw new UtilityException("Ensure you are using the full class name and that the proper assembly is accessible (e.g. is present in the executable's path or installed in the GAC)");
            }
            catch (Exception ex)
            {
                if (suppressErrors) return null;
                throw new UtilityException("Type \"" + className + "\" could not be loaded: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(Type type)
        {
            if (!type.IsClass) throw new ValidationException("may only be a reference type, detected non-class: " + type.FullName);
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Dynamically creates an instance of the supplied type
        /// </summary>
        /// <returns>A dynamically created instance of the supplied type parameter</returns>
        public static object GetInstance<T>() where T : class
        {
            return GetInstance(typeof(T));
        }

        /// <summary>
        /// Dynamically creates an instance of the class represented by the supplied class name.
        /// </summary>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="suppressErrors">Whether to return null or throw an exception for class names that are invalid or not found</param>
        /// <returns>A dynamically created instance of the supplied type</returns>
        public static object GetInstance(string className, string assemblyName = null, bool suppressErrors = false)
        {
            var type = GetType(className, assemblyName: assemblyName, suppressErrors: suppressErrors);
            if (type == null) return null;
            return GetInstance(type);
        }

        /// <summary>
        /// Dynamically creates a typed instance of the class represented by the supplied class name.
        /// </summary>
        /// <typeparam name="T">The type of the instance to be returned (e.g. a superclass)</typeparam>
        /// <param name="className">A fully qualified class name to instantiate</param>
        /// <param name="suppressErrors">Whether to return null or throw an exception for class names that are invalid or not found</param>
        /// <returns>A dynamically created PublicInstance of the supplied type</returns>
        public static T GetInstance<T>(string className, string assemblyName = null, bool suppressErrors = false) where T : class
        {
            var obj = GetInstance(className, assemblyName: assemblyName, suppressErrors: suppressErrors);
            if (obj == null) return null;
            return (T)obj;
        }

        /// <summary>
        /// Sets the value of the indicated object property
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        public static void SetInstanceProperty(object obj, string propertyName, object value, bool ignoreCase = false, bool suppressErrors = false)
        {
            var type = obj.GetType();
            var property = GetPublicInstanceProperties(type).SingleOrDefault(p => propertyName.Equals(p.Name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
            if (property != null)
            {
                try
                {
                    property.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    if (!suppressErrors)
                    {
                        var message = ex.Message;
                        int pos = message.ToUpper().IndexOf("PROPERTY ");
                        if (pos > -1)
                        {
                            message = message.Substring(0, pos + 9) + "\"" + propertyName + "\" " + message.Substring(pos + 9);
                        }
                        else
                        {
                            message += ": \"" + propertyName + "\"";
                        }
                        throw new UtilityException(message, ex);
                    }
                }
            }
            else if (!suppressErrors)
            {
                throw new UtilityException(type.Name + " does not have property \"" + propertyName + "\"" + (ignoreCase ? "" : " (hint: try setting ignoreCase = true)"));
            }
        }
    }
}
