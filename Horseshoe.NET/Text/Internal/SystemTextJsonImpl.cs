using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Text.Internal
{
    internal static class SystemTextJsonImpl
    {
        static Assembly SystemTextJson => AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name.Equals("System.Text.Json"));

        internal static string Serialize(object obj, bool indented = false)
        {
            var jsonSerializerOptionsType = SystemTextJson.GetType("System.Text.Json.JsonSerializerOptions");
            var jsonSerializerOptions = ObjectUtil.GetInstance(jsonSerializerOptionsType);
            if (indented)
            {
                jsonSerializerOptionsType
                    .GetProperty("WriteIndented", BindingFlags.Public | BindingFlags.Instance)
                    .SetValue(jsonSerializerOptions, true);
            }

            var jsonSerializerType = SystemTextJson.GetType("System.Text.Json.JsonSerializer");
            var result = jsonSerializerType
                .GetMethod("Serialize", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { obj, jsonSerializerOptions });
            return (string)result;
        }

        internal static object Deserialize(string json, Type objectType, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return default;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            var jsonSerializerType = SystemTextJson.GetType("System.Text.Json.JsonSerializer");
            var result = jsonSerializerType
                .GetMethod("Deserialize", new[] { typeof(string), typeof(Type) })
                .Invoke(null, new object[] { json, objectType });
            return result;
        }

        internal static E Deserialize<E>(string json, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return default;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            var jsonSerializerType = SystemTextJson.GetType("System.Text.Json.JsonSerializer");
            var result = jsonSerializerType
                .GetMethod("Deserialize", new[] { typeof(string) })
                .MakeGenericMethod(typeof(E))
                .Invoke(null, new object[] { json });
            return (E)result;
        }
    }
}
