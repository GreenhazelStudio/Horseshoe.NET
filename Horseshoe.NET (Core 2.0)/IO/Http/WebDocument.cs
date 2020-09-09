using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.IO.Http.Extensions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Http
{
    public static class WebDocument
    {
        public static string Get(string serviceURL, string method = "GET", object id = null, object headers = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.Method = method;
            ProcessHeaders(request, headers);

            var response = (HttpWebResponse)request.GetResponse();
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMetadata?.Invoke
                (
                    new HttpResponseMetadata
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
        }

        public static async Task<string> GetAsync(string serviceURL, string method = "GET", object id = null, object headers = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.Method = method;
            ProcessHeaders(request, headers);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMetadata?.Invoke
                (
                    new HttpResponseMetadata
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
        }

        public static E Get<E>(string serviceURL, string method = "GET", object id = null, object headers = null, bool zapBackingFields = false, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = Get(serviceURL, method: method, id: id, headers: headers, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> GetAsync<E>(string serviceURL, string method = "GET", object id = null, object headers = null, bool zapBackingFields = false, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = await GetAsync(serviceURL, method: method, id: id, headers: headers, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        static string GetFinalURL(string serviceURL, object id)
        {
            if (id != null)
            {
                if (!serviceURL.EndsWith("/"))
                {
                    serviceURL += "/";
                }
                serviceURL += id;
            }
            return serviceURL;
        }

        static void ProcessHeaders(HttpWebRequest request, object headers)
        {
            if (headers == null) return;
            if (headers is IDictionary<HttpRequestHeader, string> httpHeaders)
            {
                foreach (var kvp in httpHeaders)
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            else if (headers is IDictionary<HttpRequestHeader, string[]> httpMHeaders)
            {
                foreach (var kvp in httpMHeaders)
                {
                    request.Headers.Add(kvp.Key, string.Join(",", kvp.Value));
                }
            }
            else if (headers is IDictionary<string, string> stringHeaders)
            {
                foreach (var kvp in stringHeaders)
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }
            else if (headers is IDictionary<string, string[]> stringMHeaders)
            {
                foreach (var kvp in stringMHeaders)
                {
                    request.Headers.Add(kvp.Key, string.Join(",", kvp.Value));
                }
            }
            else
            {
                throw new ArgumentException("headers - unsupported type: " + headers.GetType().FullName + "(try one of these: IDictionary<HttpRequestHeader, string> IDictionary<HttpRequestHeader, string[]>, IDictionary<string, string>, IDictionary<string, string[]>)");
            }
        }
    }
}
