using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.WebServices
{
    public static class WebService
    {
        public static string Get(string serviceURL, string contentType, object id = null, Credential? credentials = null)
        {
            return _NonPost(serviceURL, contentType, "GET", id: id, credentials: credentials);
        }

        public static async Task<string> GetAsync(string serviceURL, string contentType, object id = null, Credential? credentials = null)
        {
            return await _NonPostAsync(serviceURL, contentType, "GET", id: id, credentials: credentials);
        }

        public static string GetJson(string serviceURL, string contentType = "application/json", object id = null, Credential? credentials = null)
        {
            return Get(serviceURL, contentType, id: id, credentials: credentials);
        }

        public static async Task<string> GetJsonAsync(string serviceURL, string contentType = "application/json", object id = null, Credential? credentials = null)
        {
            return await GetAsync(serviceURL, contentType, id: id, credentials: credentials);
        }

        public static E GetJson<E>(string serviceURL, string contentType = "application/json", object id = null, Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = GetJson(serviceURL, contentType: contentType, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }
        /*  WebApiUtil.StripBackingFields(json) */
        public static async Task<E> GetJsonAsync<E>(string serviceURL, string contentType = "application/json", object id = null, Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = await GetJsonAsync(serviceURL, contentType: contentType, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Post(string serviceURL, object content, string contentType, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            return _Post(serviceURL, content, contentType, "POST", contentSerializer: contentSerializer, credentials: credentials);
        }

        public async static Task<string> PostAsync(string serviceURL, object content, string contentType, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            return await _PostAsync(serviceURL, content, contentType, "POST", contentSerializer: contentSerializer, credentials: credentials);
        }

        public static string PostJson(string serviceURL, object content, string contentType = "application/json", Credential? credentials = null)
        {
            return Post(serviceURL, content, contentType, contentSerializer: (obj) => Serialize.Json(obj), credentials: credentials);
        }

        public static async Task<string> PostJsonAsync(string serviceURL, object content, string contentType = "application/json", Credential? credentials = null)
        {
            return await PostAsync(serviceURL, content, contentType, contentSerializer: (obj) => Serialize.Json(obj), credentials: credentials);
        }

        public static E PostJson<E>(string serviceURL, object content, string contentType = "application/json", Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = PostJson(serviceURL, content, contentType: contentType, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PostJsonAsync<E>(string serviceURL, object content, string contentType = "application/json", Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = await PostJsonAsync(serviceURL, content, contentType: contentType, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Put(string serviceURL, object content, string contentType, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            return _Post(serviceURL, content, contentType, "PUT", id: id, contentSerializer: contentSerializer, credentials: credentials);
        }

        public async static Task<string> PutAsync(string serviceURL, object content, string contentType, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            return await _PostAsync(serviceURL, content, contentType, "PUT", id: id, contentSerializer: contentSerializer, credentials: credentials);
        }

        public static string PutJson(string serviceURL, object content, string contentType = "application/json", object id = null, Credential? credentials = null)
        {
            return Put(serviceURL, content, contentType, id: id, contentSerializer: (obj) => Serialize.Json(obj), credentials: credentials);
        }

        public static async Task<string> PutJsonAsync(string serviceURL, object content, string contentType = "application/json", object id = null, Credential? credentials = null)
        {
            return await PutAsync(serviceURL, content, contentType, id: id, contentSerializer: (obj) => Serialize.Json(obj), credentials: credentials);
        }

        public static E PutJson<E>(string serviceURL, object content, string contentType = "application/json", object id = null, Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = PutJson(serviceURL, content, contentType: contentType, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PutJsonAsync<E>(string serviceURL, object content, string contentType = "application/json", object id = null, Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = await PutJsonAsync(serviceURL, content, contentType: contentType, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Delete(string serviceURL, string contentType, object content = null, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            if (content != null)
            {
                return _Post(serviceURL, content, contentType, "DELETE", id: id, contentSerializer: contentSerializer, credentials: credentials);
            }
            if (id != null)
            {
                return _NonPost(serviceURL, contentType, "DELETE", id: id, credentials: credentials);
            }
            throw new UtilityException("No id and no content was sent to 'delete' web service");
        }

        public static async Task<string> DeleteAsync(string serviceURL, string contentType, object content = null, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
        {
            if (content != null)
            {
                return await _PostAsync(serviceURL, content, contentType, "DELETE", id: id, contentSerializer: contentSerializer, credentials: credentials);
            }
            if (id != null)
            {
                return await _NonPostAsync(serviceURL, contentType, "DELETE", id: id, credentials: credentials);
            }
            throw new UtilityException("No id and no content was sent to 'delete' web service");
        }

        public static string DeleteJson(string serviceURL, string contentType = "application/json", object content = null, object id = null, Credential? credentials = null)
        {
            return Delete(serviceURL, contentType, content: content, id: id, contentSerializer: content != null ? (obj) => Serialize.Json(obj) : NoSerializer, credentials: credentials);
        }

        public static async Task<string> DeleteJsonAsync(string serviceURL, string contentType = "application/json", object content = null, object id = null, Credential? credentials = null)
        {
            return await DeleteAsync(serviceURL, contentType, content: content, id: id, contentSerializer: content != null ? (obj) => Serialize.Json(obj) : NoSerializer, credentials: credentials);
        }

        public static E DeleteJson<E>(string serviceURL, string contentType = "application/json", object content = null, object id = null, Credential? credentials = null, bool zapBackingFields = false)
        {
            var json = DeleteJson(serviceURL, contentType: contentType, content: content, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>(string serviceURL, string contentType = "application/json", object content = null, object id = null, Credential ? credentials = null, bool zapBackingFields = false)
        {
            var json = await DeleteJsonAsync(serviceURL, contentType: contentType, content: content, id: id, credentials: credentials);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        private static string _NonPost(string serviceURL, string contentType, string method, object id = null, Credential? credentials = null)
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
            credentials = credentials ?? Settings.DefaultCredentials;
            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password), credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password));
                }
                else if (credentials.Value.Password != null)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.Password, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.Password);
                }
                else
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, null as string, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, null as string);
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var rawResponse = reader.ReadToEnd();
                WebServiceUtil.RawResponseReceived?.Invoke(rawResponse);
                return rawResponse;
            }
        }

        private static async Task<string> _NonPostAsync(string serviceURL, string contentType, string method, object id = null, Credential? credentials = null)
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
            credentials = credentials ?? Settings.DefaultCredentials;
            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password), credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password));
                }
                else if (credentials.Value.Password != null)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.Password, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.Password);
                }
                else
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, null as string, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, null as string);
                }
            }

            var response = await request.GetResponseAsync() as HttpWebResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var rawResponse = reader.ReadToEnd();
                WebServiceUtil.RawResponseReceived?.Invoke(rawResponse);
                return rawResponse;
            }
        }

        private static string _Post(string serviceURL, object content, string contentType, string method, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
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
            credentials = credentials ?? Settings.DefaultCredentials;
            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password), credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password));
                }
                else if (credentials.Value.Password != null)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.Password, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.Password);
                }
                else
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, null as string, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, null as string);
                }
            }

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
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var rawResponse = reader.ReadToEnd();
                WebServiceUtil.RawResponseReceived?.Invoke(rawResponse);
                return rawResponse;
            }
        }

        private async static Task<string> _PostAsync(string serviceURL, object content, string contentType, string method, object id = null, Func<object, string> contentSerializer = null, Credential? credentials = null)
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
            credentials = credentials ?? Settings.DefaultCredentials;
            if (credentials.HasValue)
            {
                if (credentials.Value.HasSecurePassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.SecurePassword);
                }
                else if (credentials.Value.IsEncryptedPassword)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password), credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, WebServiceUtil.DecryptSecure(credentials.Value.Password));
                }
                else if (credentials.Value.Password != null)
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, credentials.Value.Password, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, credentials.Value.Password);
                }
                else
                {
                    request.Credentials = credentials.Value.Domain != null
                        ? new NetworkCredential(credentials.Value.UserID, null as string, credentials.Value.Domain)
                        : new NetworkCredential(credentials.Value.UserID, null as string);
                }
            }

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
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var rawResponse = reader.ReadToEnd();
                WebServiceUtil.RawResponseReceived?.Invoke(rawResponse);
                return rawResponse;
            }
        }

        static Func<object, string> NoSerializer { get; }
    }
}
