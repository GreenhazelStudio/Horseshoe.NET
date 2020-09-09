using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Horseshoe.NET.Text.Internal
{
    internal static class NewtonsoftJsonImpl
    {
        internal static string Serialize(object obj, bool indented = false)
        {
            var sb = new StringBuilder();
            var jsonWriter = new JsonTextWriter(new StringWriter(sb)) { Formatting = indented ? Formatting.Indented : Formatting.None };
            new JsonSerializer().Serialize(jsonWriter, obj);
            return sb.ToString();
        }

        internal static object Deserialize(string json, Type objectType = null, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return null;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            var jsonReader = new JsonTextReader(new StringReader(json));
            var obj = new JsonSerializer().Deserialize(jsonReader, objectType);
            return obj;
        }

        internal static E Deserialize<E>(string json, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return default;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            var jsonReader = new JsonTextReader(new StringReader(json));
            E obj = new JsonSerializer().Deserialize<E>(jsonReader);
            return obj;
        }
    }
}
