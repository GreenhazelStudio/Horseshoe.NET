using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.WebServices
{
    public class WSResponseHttpMeta
    {
        public int StatusCode { get; set; }
        public IDictionary<string, string[]> Headers { get; set; }
        public string Body { get; set; }
    }
}
