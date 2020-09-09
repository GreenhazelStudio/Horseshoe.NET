using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text.Internal;

namespace Horseshoe.NET.Text
{
    public static class Deserialize
    {
        public static object Json(string json, Type objectType = null, Func<string, string> preDeserializationFunc = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    return NewtonsoftJsonImpl.Deserialize(json, objectType: objectType, preDeserializationFunc: preDeserializationFunc);
                case JsonProvider.SystemTextJson:
                    if (objectType == null)
                    {
                        throw new UtilityException("The System.Text.Json provider requires 'objectType' to be specified.");
                    }
                    return SystemTextJsonImpl.Deserialize(json, objectType, preDeserializationFunc: preDeserializationFunc);
                default:
                    throw new UtilityException("Cannot find a JSON provider.  Try adding Newtonsoft.Json (a.k.a. Json.NET) or System.Text.Json");
            }
        }

        public static E Json<E>(string json, Func<string, string> preDeserializationFunc = null)
        {
            switch (TextSettings.JsonProvider)
            {
                case JsonProvider.NewtonsoftJson:
                    return NewtonsoftJsonImpl.Deserialize<E>(json, preDeserializationFunc: preDeserializationFunc);
                case JsonProvider.SystemTextJson:
                    return SystemTextJsonImpl.Deserialize<E>(json, preDeserializationFunc: preDeserializationFunc);
                default:
                    throw new UtilityException("Cannot find a JSON provider.  Try adding Newtonsoft.Json (a.k.a. Json.NET) or System.Text.Json");
            }
        }
    }
}
