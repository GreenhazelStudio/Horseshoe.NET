using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.IO.Http.Extensions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Http
{
    public static class WebService
    {
        public static string Get
        (
            string serviceURL,
            string method = "GET",
            object headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = (HttpWebResponse)request.GetResponse();
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
            }
            var responseMetadata = new HttpResponseMetadata
            {
                StatusCode = (int)response.StatusCode,
                Headers = response.Headers.ToOwinDictionary(),
                Body = rawResponse
            };
            handleResponse?.Invoke(responseMetadata);
            return rawResponse;
        }

        public static E Get<E>
        (
            string serviceURL, 
            string method = "GET", 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            var result = Get(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            
            return responseParser == null
                ? throw new ValidationException("response parser sas not supplied")
                : responseParser.Invoke(result);
        }

        public static E GetJson<E>
        (
            string serviceURL,
            string method = "GET",
            object headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            var e = Get<E>(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public static async Task<string> GetAsync
        (
            string serviceURL,
            string method = "GET",
            object headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            string rawResponse;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
            }
            var responseMetadata = new HttpResponseMetadata
            {
                StatusCode = (int)response.StatusCode,
                Headers = response.Headers.ToOwinDictionary(),
                Body = rawResponse
            };
            handleResponse?.Invoke(responseMetadata);
            return rawResponse;
        }

        public static async Task<E> GetAsync<E>
        (
            string serviceURL,
            string method = "GET", 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            var result = await GetAsync(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser sas not supplied")
                : responseParser.Invoke(result);
        }

        public static async Task<E> GetJsonAsync<E>
        (
            string serviceURL,
            string method = "GET",
            object headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            var e = await GetAsync<E>(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        // alt content type: application/x-www-form-urlencoded
        public static string Post
        (
            string serviceURL, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null, 
            string method = "POST", 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            if (content != null || contentSerializer != null)
            {
                string serialized = contentSerializer != null
                    ? contentSerializer.Invoke(content)
                    : content?.ToString();

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
            }
            var responseMetadata= new HttpResponseMetadata
            {
                StatusCode = (int)response.StatusCode,
                Headers = response.Headers.ToOwinDictionary(),
                Body = rawResponse
            };
            handleResponse?.Invoke(responseMetadata);
            return rawResponse;
        }

        public async static Task<string> PostAsync
        (
            string serviceURL, 
            object content, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            string method = "POST", 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol ?? (SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
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
                handleResponseMetadata?.Invoke
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

        public static E Post<E>
        (
            string serviceURL, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST", 
            object headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            var json = Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PostAsync<E>
        (
            string serviceURL, 
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST", 
            object headers = null, 
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            var json = await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Put
        (
            string serviceURL, 
            object content, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
        }

        public async static Task<string> PutAsync
        (
            string serviceURL, 
            object content, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
        }

        public static E Put<E>
        (
            string serviceURL, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null,
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var json = Put(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> PutAsync<E>
        (
            string serviceURL, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null,
            object id = null, 
            object headers = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var json = await PutAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static string Delete
        (
            string serviceURL, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            return content != null
                ? Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata)
                : Get(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
        }

        public static async Task<string> DeleteAsync
        (
            string serviceURL, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            return content != null
                ? await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata)
                : await GetAsync(serviceURL, method: "DELETE", headers: headers, id: id, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
        }

        public static E Delete<E>
        (
            string serviceURL, 
            object content = null, 
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            object headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            var json = content != null
                ? Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata)
                : Get(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
            var e = zapBackingFields
                ? Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields)
                : Deserialize.Json<E>(json);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>
        (
            string serviceURL, 
            object content = null, 
            string contentType = null,
            Func<object, string> contentSerializer = null,
            object headers = null, 
            object id = null, 
            Credential ? credentials = null,
            SecurityProtocolType? securityProtocol = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false, 
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata> handleResponseMetadata = null
        )
        {
            var json = content != null
                ? await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata)
                : await GetAsync(serviceURL, method: "DELETE", headers: headers, id: id, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponseMetadata: handleResponseMetadata);
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

        internal static void ProcessHeaders(HttpWebRequest request, object headers)
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

        internal static void ProcessCredentials(HttpWebRequest request, Credential? credentials)
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

        internal static void ProcessCertificates(HttpWebRequest request, string certificatePath, string certificatePfxPath, string certificatePfxPassword, X509KeyStorageFlags? certificateX509KeyStorageFlags)
        {
            if (certificatePath == null && certificatePfxPath == null) return;
            var certificates = new X509Certificate2Collection();
            if (certificatePath != null)
            {
                if (certificatePfxPath != null) throw new ValidationException("Please supply only a certificate or pfx, not both");
                certificates.Import(certificatePath);
            }
            else if (certificatePfxPath != null)
            {
                if (certificatePfxPassword == null) throw new ValidationException("Please supply the pfx password");
                certificates.Import(certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags ?? X509KeyStorageFlags.DefaultKeySet);
            }
            request.ClientCertificates = certificates;
        }

        private static Func<string, E> GetJsonDeserializer<E>(bool zapBackingFields)
        {
            if (zapBackingFields)
            {
                return (json) => Deserialize.Json<E>(json, preDeserializationFunc: WebServiceUtil.ZapBackingFields);
            }
            return (json) => Deserialize.Json<E>(json);
        }
    }
}
