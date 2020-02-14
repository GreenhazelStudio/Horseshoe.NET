using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Horseshoe.NET.Text
{
    public static class Deserialize
    {
        public static object Json(string json, Func<string, string> preDeserializationFunc = null)
        {
            if (json == null) return null;
            if (preDeserializationFunc != null)
            {
                json = preDeserializationFunc.Invoke(json);
            }
            var jsonReader = new JsonTextReader(new StringReader(json));
            object obj = new JsonSerializer().Deserialize(jsonReader);
            return obj;
        }

        public static E Json<E>(string json, Func<string, string> preDeserializationFunc = null)
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
