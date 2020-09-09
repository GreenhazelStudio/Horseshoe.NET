using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Horseshoe.NET.Text.Internal
{
    internal static class SystemTextJsonImpl
    {
        internal static string Serialize(object obj, bool indented = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = indented };
            return JsonSerializer.Serialize(obj, options);
        }

        internal static object Deserialize(string json, Type objectType, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return default;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            return JsonSerializer.Deserialize(json, objectType);
        }

        internal static E Deserialize<E>(string json, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return default;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            return JsonSerializer.Deserialize<E>(json);
        }
    }
}
