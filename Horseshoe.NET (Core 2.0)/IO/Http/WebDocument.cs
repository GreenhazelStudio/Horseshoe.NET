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
        public static byte[] GetBytes(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var bytes = new List<byte>();
            var buf = new byte[1024];
            using (var stream = GetStream(documentURL, method: method, id: id, headers: headers, credentials: credentials, returnMetadata: returnMetadata))
            {
                while(true)
                {
                    var result = stream.Read(buf, 0, buf.Length);
                    if (result == buf.Length)
                    {
                        bytes.AddRange(buf);
                    }
                    else if (result > 0)
                    {
                        var minibuf = new byte[0];
                        Array.Copy(buf, minibuf, result);
                        bytes.AddRange(minibuf);
                    }
                    else break;
                }
            }
            return bytes.ToArray();
        }

        public static Stream GetStream(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            documentURL = GetFinalURL(documentURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (documentURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(documentURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            WebService.ProcessHeaders(request, headers);
            WebService.ProcessCredentials(request, credentials);
            customizeRequest?.Invoke(request);

            var response = (HttpWebResponse)request.GetResponse();
            returnMetadata?.Invoke
            (
                new HttpResponseMetadata
                {
                    StatusCode = (int)response.StatusCode,
                    Headers = response.Headers.ToOwinDictionary(),
                    Body = null
                }
            );
            return response.GetResponseStream();
        }

        public static string Get(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            documentURL = GetFinalURL(documentURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (documentURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(documentURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            WebService.ProcessHeaders(request, headers);
            WebService.ProcessCredentials(request, credentials);
            customizeRequest?.Invoke(request);

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

        public static async Task<string> GetAsync(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            documentURL = GetFinalURL(documentURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (documentURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(documentURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            WebService.ProcessHeaders(request, headers);
            WebService.ProcessCredentials(request, credentials);
            customizeRequest?.Invoke(request);

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

        public static E Get<E>(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, bool zapBackingFields = false, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = Get(documentURL, method: method, id: id, headers: headers, credentials: credentials, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> GetAsync<E>(string documentURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, bool zapBackingFields = false, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = await GetAsync(documentURL, method: method, id: id, headers: headers, credentials: credentials, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        static string GetFinalURL(string documentURL, object id)
        {
            if (id != null)
            {
                if (!documentURL.EndsWith("/"))
                {
                    documentURL += "/";
                }
                documentURL += id;
            }
            return documentURL;
        }
    }
}
