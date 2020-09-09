using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    public static class Serialize
    {
        public static event JsonGenerated JsonGenerated;

        public static string Json(object obj, bool indented = true)
        {
            string json;
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    json = NewtonsoftJsonImpl.Serialize(obj, indented: indented);
                    JsonGenerated?.Invoke(json);
                    return json;
                case JsonProvider.SystemTextJson:
                    json = SystemTextJsonImpl.Serialize(obj, indented: indented);
                    JsonGenerated?.Invoke(json);
                    return json;
                default:
                    throw new UtilityException("Cannot find a JSON provider.  Try adding Newtonsoft.Json (a.k.a. Json.NET) or System.Text.Json");
            }
        }
    }
}
