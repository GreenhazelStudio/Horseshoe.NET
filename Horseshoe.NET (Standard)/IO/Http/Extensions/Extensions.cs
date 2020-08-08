using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.Http.Extensions
{
    public static class Extensions
    {
        public static IDictionary<string, string[]> ToOwinDictionary(this WebHeaderCollection collection)
        {
            if (collection == null) return null;
            var dict = new Dictionary<string, string[]>();
            for (int i = 0; i < collection.Count; i++) 
            {
                var name = collection.GetKey(i);
                var values = collection.GetValues(name);
                if (!values.Any()) continue;
                dict.Add(name, values);
            }
            return dict;
        }
    }
}
