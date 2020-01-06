using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Horseshoe.NET.Objects
{
    public static class Extensions
    {
        public static PropertyInfo[] GetProperties(this object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetProperties(obj.GetType(), bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetPublicStaticProperties(this Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPublicStaticProperties(type, bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo[] GetPublicInstanceProperties(this object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPublicInstanceProperties(obj.GetType(), bindingFlags: bindingFlags, filter: filter);
        }

        public static PropertyInfo GetProperty(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetProperty(obj.GetType(), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetPublicStaticProperty(this Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicStaticProperty(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static PropertyInfo GetPublicInstanceProperty(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicInstanceProperty(obj.GetType(), name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue[] GetPropertyValues(this object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPropertyValues(obj, bindingFlags: bindingFlags, filter: filter);
        }

        public static IPropertyValue[] GetPublicStaticPropertyValues(this Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPublicStaticPropertyValues(type, bindingFlags: bindingFlags, filter: filter);
        }

        public static IPropertyValue<P>[] GetPublicStaticPropertyValuesOfType<P>(this Type type, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPublicStaticPropertyValuesOfType<P>(type, bindingFlags: bindingFlags, filter: filter);
        }

        public static IPropertyValue[] GetPublicInstancePropertyValues(this object obj, BindingFlags? bindingFlags = null, Func<PropertyInfo, bool> filter = null)
        {
            return ObjectUtil.GetPublicInstancePropertyValues(obj, bindingFlags: bindingFlags, filter: filter);
        }

        public static IPropertyValue<P> GetPropertyValue<P>(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPropertyValue<P>(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue GetPropertyValue(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPropertyValue(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue<P> GetPublicStaticPropertyValue<P>(this Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicStaticPropertyValue<P>(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue GetPublicStaticPropertyValue(this Type type, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicStaticPropertyValue(type, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue<P> GetPublicInstancePropertyValue<P>(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicInstancePropertyValue<P>(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue GetPublicInstancePropertyValue(this object obj, string name, bool ignoreCase = false, BindingFlags? bindingFlags = null)
        {
            return ObjectUtil.GetPublicInstancePropertyValue(obj, name, ignoreCase: ignoreCase, bindingFlags: bindingFlags);
        }

        public static IPropertyValue<P> GetNestedPropertyValue<P>(this object obj, string propertyFullPath, bool ignoreCase = false)
        {
            return ObjectUtil.GetNestedPropertyValue<P>(obj, propertyFullPath, ignoreCase: ignoreCase);
        }

        public static IPropertyValue GetNestedPropertyValue(this object obj, string propertyFullPath, bool ignoreCase = false)
        {
            return ObjectUtil.GetNestedPropertyValue(obj, propertyFullPath, ignoreCase: ignoreCase);
        }

        public static void SetInstanceProperty(this object obj, string propertyName, object value, bool ignoreCase = false)
        {
            ObjectUtil.SetInstanceProperty(obj, propertyName, value, ignoreCase: ignoreCase);
        }

        public static T Copy<T>(this T obj) where T : class, new()
        {
            var t = new T();
            ObjectUtil.EasyMap(obj, t);
            return t;
        }

        public static T DeepCopy<T>(this T obj) where T : class, new()
        {
            var t = new T();
            ObjectUtil.EasyMap(obj, t, deepCopy: true);
            return t;
        }
    }
}
