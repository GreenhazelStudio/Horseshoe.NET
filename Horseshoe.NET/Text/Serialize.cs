using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Events;

using Newtonsoft.Json;

namespace Horseshoe.NET.Text
{
    public static class Serialize
    {
        public static event EasyNotifier<string> JsonPayloadGenerated;

        public static string Json(object obj)
        {
            var sb = new StringBuilder();
            var jsonWriter = new JsonTextWriter(new StringWriter(sb));
            new JsonSerializer().Serialize(jsonWriter, obj);
            JsonPayloadGenerated?.Invoke(sb.ToString());
            return sb.ToString();
        }
    }
}
