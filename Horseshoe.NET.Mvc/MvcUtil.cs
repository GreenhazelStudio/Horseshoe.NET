using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Horseshoe.NET.Mvc
{
    public static class MvcUtil
    {
        public static string GetRequestBody(HttpContextBase context)
        {
            using (var stream = new MemoryStream())
            {
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                context.Request.InputStream.CopyTo(stream);
                var rawBody = Encoding.UTF8.GetString(stream.ToArray());
                return rawBody;
            }
        }

        public static string GetRequestBody(HttpRequestMessage mvcRequest)
        {
            var context = (HttpContextBase)mvcRequest.Properties["MS_HttpContext"];
            return GetRequestBody(context);
        }
    }
}
