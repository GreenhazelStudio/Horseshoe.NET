using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.WebServices
{
    public static class WebService
    {
        public static string Get(string serviceURL, string contentType, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return NonPost(serviceURL, contentType, "GET", id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static async Task<string> GetAsync(string serviceURL, string contentType, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await NonPostAsync(serviceURL, contentType, "GET", id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static string GetJson(string serviceURL, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Get(serviceURL, contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static async Task<string> GetJsonAsync(string serviceURL, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await GetAsync(serviceURL, contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static E GetJson<E>(string serviceURL, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = GetJson(serviceURL, contentType: contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }
        /*  WebApiUtil.StripBackingFields(json) */
        public static async Task<E> GetJsonAsync<E>(string serviceURL, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = await GetJsonAsync(serviceURL, contentType: contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Post(string serviceURL, object content, string contentType, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Post(serviceURL, content, contentType, "POST", contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public async static Task<string> PostAsync(string serviceURL, object content, string contentType, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await PostAsync(serviceURL, content, contentType, "POST", contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static string PostJson(string serviceURL, object content, string contentType = "application/json", object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Post(serviceURL, content, contentType, contentSerializer: (obj) => Serialize.Json(obj), headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static async Task<string> PostJsonAsync(string serviceURL, object content, string contentType = "application/json", object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await PostAsync(serviceURL, content, contentType, contentSerializer: (obj) => Serialize.Json(obj), headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static E PostJson<E>(string serviceURL, object content, string contentType = "application/json", object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = PostJson(serviceURL, content, contentType: contentType, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PostJsonAsync<E>(string serviceURL, object content, string contentType = "application/json", object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = await PostJsonAsync(serviceURL, content, contentType: contentType, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Put(string serviceURL, object content, string contentType, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Post(serviceURL, content, contentType, "PUT", id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public async static Task<string> PutAsync(string serviceURL, object content, string contentType, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await PostAsync(serviceURL, content, contentType, "PUT", id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static string PutJson(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Put(serviceURL, content, contentType, id: id, contentSerializer: (obj) => Serialize.Json(obj), headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static async Task<string> PutJsonAsync(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await PutAsync(serviceURL, content, contentType, id: id, contentSerializer: (obj) => Serialize.Json(obj), headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static E PutJson<E>(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = PutJson(serviceURL, content, contentType: contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PutJsonAsync<E>(string serviceURL, object content, string contentType = "application/json", object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = await PutJsonAsync(serviceURL, content, contentType: contentType, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Delete(string serviceURL, string contentType, object content = null, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (content != null)
            {
                return Post(serviceURL, content, contentType, "DELETE", id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
            }
            if (id != null)
            {
                return NonPost(serviceURL, contentType, "DELETE", id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            }
            throw new UtilityException("No id and no content was sent to 'delete' web service");
        }

        public static async Task<string> DeleteAsync(string serviceURL, string contentType, object content = null, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (content != null)
            {
                return await PostAsync(serviceURL, content, contentType, "DELETE", id: id, contentSerializer: contentSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
            }
            if (id != null)
            {
                return await NonPostAsync(serviceURL, contentType, "DELETE", id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            }
            throw new UtilityException("No id and no content was sent to 'delete' web service");
        }

        public static string DeleteJson(string serviceURL, string contentType = "application/json", object content = null, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return Delete(serviceURL, contentType, content: content, id: id, contentSerializer: content != null ? (obj) => Serialize.Json(obj) : NoSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static async Task<string> DeleteJsonAsync(string serviceURL, string contentType = "application/json", object content = null, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            return await DeleteAsync(serviceURL, contentType, content: content, id: id, contentSerializer: content != null ? (obj) => Serialize.Json(obj) : NoSerializer, headers: headers, credentials: credentials, returnMeta: returnMeta);
        }

        public static E DeleteJson<E>(string serviceURL, string contentType = "application/json", object content = null, object id = null, object headers = null, Credential? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = DeleteJson(serviceURL, contentType: contentType, content: content, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>(string serviceURL, string contentType = "application/json", object content = null, object id = null, object headers = null, Credential ? credentials = null, bool zapBackingFields = false, Action<WSResponseHttpMeta> returnMeta = null)
        {
            var json = await DeleteJsonAsync(serviceURL, contentType: contentType, content: content, id: id, headers: headers, credentials: credentials, returnMeta: returnMeta);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string NonPost(string serviceURL, string contentType, string method, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (id != null)
            {
                if (!serviceURL.EndsWith("/"))
                {
                    serviceURL += "/";
                }
                serviceURL += id;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);

            var response = (HttpWebResponse)request.GetResponse();
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMeta?.Invoke
                (
                    new WSResponseHttpMeta
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
        }

        public static async Task<string> NonPostAsync(string serviceURL, string contentType, string method, object id = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (id != null)
            {
                if (!serviceURL.EndsWith("/"))
                {
                    serviceURL += "/";
                }
                serviceURL += id;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMeta?.Invoke
                (
                    new WSResponseHttpMeta
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
        }

        public static string Post(string serviceURL, object content, string contentType, string method, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (id != null)
            {
                if (!serviceURL.EndsWith("/"))
                {
                    serviceURL += "/";
                }
                serviceURL += id;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                if (contentSerializer != null)
                {
                    var serialized = contentSerializer.Invoke(content);
                    streamWriter.Write(serialized);
                }
                else if (content != null)
                {
                    streamWriter.Write(content.ToString());
                }
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMeta?.Invoke
                (
                    new WSResponseHttpMeta
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
        }

        public async static Task<string> PostAsync(string serviceURL, object content, string contentType, string method, object id = null, Func<object, string> contentSerializer = null, object headers = null, Credential? credentials = null, Action<WSResponseHttpMeta> returnMeta = null)
        {
            if (id != null)
            {
                if (!serviceURL.EndsWith("/"))
                {
                    serviceURL += "/";
                }
                serviceURL += id;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                if (contentSerializer != null)
                {
                    var serialized = contentSerializer.Invoke(content);
                    streamWriter.Write(serialized);
                }
                else if (content != null)
                {
                    streamWriter.Write(content.ToString());
                }
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
                returnMeta?.Invoke
                (
                    new WSResponseHttpMeta
                    {
                        StatusCode = (int)response.StatusCode,
                        Headers = response.Headers.ToOwinDictionary(),
                        Body = rawResponse
                    }
                );
            }
            return rawResponse;
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
            credentials = credentials ?? WebServiceSettings.DefaultCredentials;
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

        static Func<object, string> NoSerializer { get; }
    }
}
