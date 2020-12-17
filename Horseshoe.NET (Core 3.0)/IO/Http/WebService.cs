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
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = request.GetResponse() as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            if (handleResponse != null)
            {
                var responseMetadata = new HttpResponseMetadata(response.StatusCode, response.StatusDescription, response.Headers.ToOwinDictionary());
                handleResponse.Invoke(responseMetadata, responseStream);
                responseStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static E Get<E>
        (
            string serviceURL, 
            string method = "GET", 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = Get(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static E GetJson<E>
        (
            string serviceURL,
            string method = "GET",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = Get<E>(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public static async Task<string> GetAsync
        (
            string serviceURL,
            string method = "GET",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            if (handleResponse != null)
            {
                var responseMetadata = new HttpResponseMetadata(response.StatusCode, response.StatusDescription, response.Headers.ToOwinDictionary());
                handleResponse.Invoke(responseMetadata, responseStream);
                responseStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<E> GetAsync<E>
        (
            string serviceURL,
            string method = "GET", 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = await GetAsync(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static async Task<E> GetJsonAsync<E>
        (
            string serviceURL,
            string method = "GET",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await GetAsync<E>(serviceURL, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        // alt content type: application/json
        // alt content type: application/x-www-form-urlencoded
        public static string Post
        (
            string serviceURL, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null, 
            string method = "POST", 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessContent(request, content, contentSerializer);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = request.GetResponse() as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            if (handleResponse != null)
            {
                var responseMetadata = new HttpResponseMetadata(response.StatusCode, response.StatusDescription, response.Headers.ToOwinDictionary());
                handleResponse.Invoke(responseMetadata, responseStream);
                responseStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static E Post<E>
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static E PostJson<E>
        (
            string serviceURL,
            object content,
            string method = "POST",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = Post<E>(serviceURL, content, contentType: "application/json", contentSerializer: JsonContentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public async static Task<string> PostAsync
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            serviceURL = GetFinalURL(serviceURL, id);
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceURL.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceURL);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.ContentType = contentType;
            request.Method = method;
            ProcessHeaders(request, headers);
            ProcessContent(request, content, contentSerializer);
            ProcessCredentials(request, credentials);
            ProcessCertificates(request, certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            customizeRequest?.Invoke(request);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            var responseStream = response.GetResponseStream();
            if (handleResponse != null)
            {
                var responseMetadata = new HttpResponseMetadata(response.StatusCode, response.StatusDescription, response.Headers.ToOwinDictionary());
                handleResponse.Invoke(responseMetadata, responseStream);
                responseStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static async Task<E> PostAsync<E>
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static async Task<E> PostJsonAsync<E>
        (
            string serviceURL,
            object content,
            string method = "POST",
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostAsync<E>(serviceURL, content, contentType: "application/json", contentSerializer: JsonContentSerializer, method: method, headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public static string Put
        (
            string serviceURL, 
            object content, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static E Put<E>
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var e = Post<E>(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: responseParser, handleResponse: handleResponse);
            return e;
        }

        public static E PutJson<E>
        (
            string serviceURL,
            object content,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var e = PostJson<E>(serviceURL, content, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public async static Task<string> PutAsync
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static async Task<E> PutAsync<E>
        (
            string serviceURL,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            object id = null,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostAsync<E>(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, responseParser: responseParser, handleResponse: handleResponse);
            return e;
        }

        public static async Task<E> PutJsonAsync<E>
        (
            string serviceURL,
            object content,
            object id = null,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostJsonAsync<E>(serviceURL, content, method: "PUT", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static string Delete
        (
            string serviceURL, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            return content != null
                ? Post(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : Get(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static E Delete<E>
        (
            string serviceURL,
            object content = null,
            string contentType = "application/json",
            Func<object, string> contentSerializer = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? Post<E>(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : Get<E>(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static E DeleteJson<E>
        (
            string serviceURL,
            object content = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? PostJson<E>(serviceURL, content, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : GetJson<E>(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static async Task<string> DeleteAsync
        (
            string serviceURL, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            IDictionary<object,string> headers = null, 
            object id = null, 
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            return content != null
                ? await PostAsync(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetAsync(serviceURL, method: "DELETE", headers: headers, id: id, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static async Task<E> DeleteAsync<E>
        (
            string serviceURL,
            object content = null,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? await PostAsync<E>(serviceURL, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetAsync<E>(serviceURL, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>
        (
            string serviceURL,
            object content = null,
            IDictionary<object,string> headers = null,
            object id = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? await PostJsonAsync<E>(serviceURL, content, method: "DELETE", headers: headers, id: id, credentials: credentials, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetJsonAsync<E>(serviceURL, method: "DELETE", headers: headers, id: id, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
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

        internal static void ProcessHeaders(HttpWebRequest request, IDictionary<object,string> headers)
        {
            if (headers == null) return;
            foreach (object key in headers.Keys)
            {
                if (key is string stringKey)
                {
                    request.Headers.Add(stringKey, headers[key]);
                }
                else if (key is HttpRequestHeader requestHeaderKey)
                {
                    request.Headers.Add(requestHeaderKey, headers[key]);
                }
                else if (key is HttpResponseHeader responseHeaderKey)
                {
                    request.Headers.Add(responseHeaderKey, headers[key]);
                }
                else
                {
                    request.Headers.Add(key.ToString(), headers[key]);
                }
            }
        }

        internal static void ProcessContent(HttpWebRequest request, object content, Func<object, string> contentSerializer)
        {
            if (content != null)
            {
                string _content;
                if (contentSerializer != null)
                {
                    _content = contentSerializer.Invoke(content);
                }
                else
                {
                    _content = content.ToString();
                }
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(_content);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
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

        private static Func<object, string> JsonContentSerializer => (obj) => Serialize.Json(obj);
    }
}
