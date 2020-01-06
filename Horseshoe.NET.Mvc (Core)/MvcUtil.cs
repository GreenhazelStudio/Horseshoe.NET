using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Http;

namespace Horseshoe.NET.Mvc
{
    public static class MvcUtil
    {
        public static string GetRequestBody(HttpRequest mvcRequest)
        {
            using (var stream = new MemoryStream())
            {
                mvcRequest.Body.Seek(0, SeekOrigin.Begin);
                mvcRequest.Body.CopyTo(stream);
                var rawBody = Encoding.UTF8.GetString(stream.ToArray());
                return rawBody;
            }
        }

        public static string GetRequestBody(HttpContext context)
        {
            return GetRequestBody(context.Request);
        }
    }
}
