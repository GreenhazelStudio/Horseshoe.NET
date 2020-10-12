using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.IO.Http.Extensions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Http
{
    public static class WebService
    {
        public static string Get(string serviceURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
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

        public static async Task<string> GetAsync(string serviceURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
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

        public static E Get<E>(string serviceURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = Get(serviceURL, method: method, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> GetAsync<E>(string serviceURL, string method = "GET", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = await GetAsync(serviceURL, method: method, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        // alt content type: application/x-www-form-urlencoded
        public static string Post(string serviceURL, string method = "POST", object content = null, string contentType = "application/json", object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            customizeRequest?.Invoke(request);

            if (content != null || contentSerializer != null)
            {
                string serialized = "";
                if (contentSerializer != null)
                {
                    serialized = contentSerializer.Invoke(content);
                }
                else if (content != null)
                {
                    if (contentType.ToLower().Contains("json"))
                    {
                        serialized = Serialize.Json(content);
                    }
                    else
                    {
                        serialized = content.ToString();
                    }
                }
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(serialized);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

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

        public async static Task<string> PostAsync(string serviceURL, string method = "POST", object content = null, string contentType = "application/json", object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, SecurityProtocolType? securityProtocol = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            customizeRequest?.Invoke(request);

            if (content != null || contentSerializer != null)
            {
                string serialized = "";
                if (contentSerializer != null)
                {
                    serialized = contentSerializer.Invoke(content);
                }
                else if (content != null)
                {
                    if (contentType.ToLower().Contains("json"))
                    {
                        serialized = Serialize.Json(content);
                    }
                    else
                    {
                        serialized = content.ToString();
                    }
                }
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(serialized);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

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

        public static E Post<E>(string serviceURL, string method = "POST", object content = null, string contentType = "application/json", object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = Post(serviceURL, method: method, content: content, contentType: contentType, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PostAsync<E>(string serviceURL, string method = "POST", object content = null, string contentType = "application/json", object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = await PostAsync(serviceURL, method: method, content: content, contentType: contentType, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Put(string serviceURL, object content, string contentType = "application/json", object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return Post(serviceURL, method: "PUT", content: content, contentType: contentType, id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
        }

        public async static Task<string> PutAsync(string serviceURL, object content, string contentType = "application/json", object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return await PostAsync(serviceURL, method: "PUT", content: content, contentType: contentType, id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
        }

        public static E Put<E>(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var json = Put(serviceURL, content, contentType: contentType, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PutAsync<E>(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var json = await PutAsync(serviceURL, content, contentType: contentType, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Delete(string serviceURL, object content = null, string contentType = "application/json", object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content != null)
            {
                return Post(serviceURL, method: "DELETE", content: content, contentType: contentType, id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            }
            return Get(serviceURL, method: "DELETE", id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
        }

        public static async Task<string> DeleteAsync(string serviceURL, string contentType = "application/json", object content = null, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            if (content != null)
            {
                return await PostAsync(serviceURL, method: "DELETE", content: content, contentType: contentType, id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            }
            return await GetAsync(serviceURL, method: "DELETE", id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
        }

        public static E Delete<E>(string serviceURL, object content = null, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = Delete(serviceURL, content: content, contentType: contentType, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>(string serviceURL, object content = null, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<HttpWebRequest> customizeRequest = null, Action<HttpResponseMetadata> returnMetadata = null)
        {
            var json = await DeleteAsync(serviceURL, content: content, contentType: contentType, id: id, headers: headers, credentials: credentials, customizeRequest: customizeRequest, returnMetadata: returnMetadata);
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

        static void ProcessCredentials(HttpWebRequest request, Credential? credentials)
        {
            credentials = credentials ?? WebServiceSettings.DefaultWebServiceCredentials;
            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserName, credentials.Value.SecurePassword, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserName, credentials.Value.SecurePassword);
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserName, Decrypt.SecureString(credentials.Value.Password), credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserName, Decrypt.SecureString(credentials.Value.Password));
                }
                else if (credentials.Value.Password != null)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserName, credentials.Value.Password, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserName, credentials.Value.Password);
                }
                else
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserName, null as string, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserName, null as string);
                }
            }
        }
    }
}
