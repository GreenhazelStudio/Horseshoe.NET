using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.Http
{
    public class HttpResponseMetadata
    {
        public HttpStatusCode StatusCode { get; }
        public string StatusText => StatusCode.ToString();
        public string StatusDescription { get; }
        public IDictionary<string, string[]> Headers { get; }
        public string ContentType
        {
            get
            {
                var contentTypeHeader = Headers.Keys.FirstOrDefault(k => k.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
                if (contentTypeHeader != null) return Headers[contentTypeHeader].FirstOrDefault();
                return null;
            }
        }
        public bool KeepStreamOpen { get; set; }

        internal HttpResponseMetadata (HttpStatusCode statusCode, string statusDescription, IDictionary<string, string[]> headers)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            Headers = headers;
        }
    }
}
