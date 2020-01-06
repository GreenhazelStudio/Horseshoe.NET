using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Objects
{
    public interface IPropertyValue<T>
    {
        PropertyInfo Property { get; }
        T Value { get; }
    }

    public interface IPropertyValue : IPropertyValue<object>
    {
    }
}
