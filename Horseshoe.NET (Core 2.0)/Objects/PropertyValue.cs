using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Objects
{
    public struct PropertyValue : IPropertyValue
    {
        public PropertyInfo Property { get; }
        public object Value { get; }

        public PropertyValue(PropertyInfo property, object value)
        {
            Property = property;
            Value = value;
        }
    }

    public struct PropertyValue<T> : IPropertyValue<T>
    {
        public PropertyInfo Property { get; }
        public T Value { get; }

        public PropertyValue(PropertyInfo property, T value)
        {
            Property = property;
            Value = value;
        }
    }
}
