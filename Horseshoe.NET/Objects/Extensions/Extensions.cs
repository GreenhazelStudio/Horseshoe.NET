using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.Objects.Extensions
{
    public static class Extensions
    {
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
