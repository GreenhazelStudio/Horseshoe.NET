using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.IO.Http.Extensions;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Http
{
    public static class WebService
    {
        public static string Get
        (
            string serviceUrl,
            string method = "GET",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null, 
            int? proxyPort = null, 
            bool proxyBypassOnLocal = false, 
            string[] proxyBypassList = null, 
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var request = GetWebRequest(serviceUrl, method, securityProtocol);
            request.ProcessHeaders(headers);
            request.ProcessCredentials(credentials);
            request.ProcessCertificates(certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            request.ProcessProxy(proxyAddress, proxyPort, proxyBypassOnLocal, proxyBypassList, proxyCredentials);
            customizeRequest?.Invoke(request);

            var response = request.GetResponse() as HttpWebResponse;
            return response.ProcessResponse(handleResponse);
        }

        public static E Get<E>
        (
            string serviceUrl, 
            string method = "GET", 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = Get(serviceUrl, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
            
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static E GetJson<E>
        (
            string serviceUrl,
            string method = "GET",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = Get<E>(serviceUrl, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public static async Task<string> GetAsync
        (
            string serviceUrl,
            string method = "GET",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var request = GetWebRequest(serviceUrl, method, securityProtocol);
            request.ProcessHeaders(headers);
            request.ProcessCredentials(credentials);
            request.ProcessCertificates(certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            request.ProcessProxy(proxyAddress, proxyPort, proxyBypassOnLocal, proxyBypassList, proxyCredentials);
            customizeRequest?.Invoke(request);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            return await response.ProcessResponseAsync(handleResponse);
        }

        public static async Task<E> GetAsync<E>
        (
            string serviceUrl,
            string method = "GET", 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = await GetAsync(serviceUrl, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static async Task<E> GetJsonAsync<E>
        (
            string serviceUrl,
            string method = "GET",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await GetAsync<E>(serviceUrl, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        // alt content type: application/json
        // alt content type: application/x-www-form-urlencoded
        public static string Post
        (
            string serviceUrl, 
            object content, 
            string contentType = null,
            Func<object, string> contentSerializer = null, 
            string method = "POST", 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var request = GetWebRequest(serviceUrl, method, securityProtocol);
            request.ProcessContent(content, contentSerializer);
            request.ProcessHeaders(headers, contentType);
            request.ProcessCredentials(credentials);
            request.ProcessCertificates(certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            request.ProcessProxy(proxyAddress, proxyPort, proxyBypassOnLocal, proxyBypassList, proxyCredentials);
            customizeRequest?.Invoke(request);

            var response = request.GetResponse() as HttpWebResponse;
            return response.ProcessResponse(handleResponse);
        }

        public static E Post<E>
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = Post(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static E PostJson<E>
        (
            string serviceUrl,
            object content,
            string method = "POST",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = Post<E>(serviceUrl, content, contentType: "application/json", contentSerializer: JsonContentSerializer, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public async static Task<string> PostAsync
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var request = GetWebRequest(serviceUrl, method, securityProtocol);
            request.ProcessContent(content, contentSerializer);
            request.ProcessHeaders(headers, contentType);
            request.ProcessCredentials(credentials);
            request.ProcessCertificates(certificatePath, certificatePfxPath, certificatePfxPassword, certificateX509KeyStorageFlags);
            request.ProcessProxy(proxyAddress, proxyPort, proxyBypassOnLocal, proxyBypassList, proxyCredentials);
            customizeRequest?.Invoke(request);

            var response = await request.GetResponseAsync() as HttpWebResponse;
            return await response.ProcessResponseAsync(handleResponse);
        }

        public static async Task<E> PostAsync<E>
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            string method = "POST",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var result = await PostAsync(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return responseParser == null
                ? throw new ValidationException("response parser was not supplied")
                : responseParser.Invoke(result);
        }

        public static async Task<E> PostJsonAsync<E>
        (
            string serviceUrl,
            object content,
            string method = "POST",
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostAsync<E>(serviceUrl, content, contentType: "application/json", contentSerializer: JsonContentSerializer, method: method, securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: GetJsonDeserializer<E>(zapBackingFields), handleResponse: handleResponse);
            return e;
        }

        public static string Put
        (
            string serviceUrl, 
            object content, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return Post(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static E Put<E>
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var e = Post<E>(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: responseParser, handleResponse: handleResponse);
            return e;
        }

        public static E PutJson<E>
        (
            string serviceUrl,
            object content,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            var e = PostJson<E>(serviceUrl, content, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public async static Task<string> PutAsync
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            if (content == null) throw new ArgumentNullException(nameof(content), "cannot be null");
            return await PostAsync(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static async Task<E> PutAsync<E>
        (
            string serviceUrl,
            object content,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Func<string, E> responseParser = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostAsync<E>(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, responseParser: responseParser, handleResponse: handleResponse);
            return e;
        }

        public static async Task<E> PutJsonAsync<E>
        (
            string serviceUrl,
            object content,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = await PostJsonAsync<E>(serviceUrl, content, method: "PUT", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static string Delete
        (
            string serviceUrl, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            return content != null
                ? Post(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : Get(serviceUrl, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static E Delete<E>
        (
            string serviceUrl,
            object content = null,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? Post<E>(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : Get<E>(serviceUrl, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static E DeleteJson<E>
        (
            string serviceUrl,
            object content = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? PostJson<E>(serviceUrl, content, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : GetJson<E>(serviceUrl, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static async Task<string> DeleteAsync
        (
            string serviceUrl, 
            object content = null, 
            string contentType = null, 
            Func<object, string> contentSerializer = null, 
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null, 
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null, 
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            return content != null
                ? await PostAsync(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetAsync(serviceUrl, method: "DELETE", headers: headers, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
        }

        public static async Task<E> DeleteAsync<E>
        (
            string serviceUrl,
            object content = null,
            string contentType = null,
            Func<object, string> contentSerializer = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? await PostAsync<E>(serviceUrl, content, contentType: contentType, contentSerializer: contentSerializer, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetAsync<E>(serviceUrl, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        public static async Task<E> DeleteJsonAsync<E>
        (
            string serviceUrl,
            object content = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12,
            IDictionary<object,string> headers = null,
            Credential? credentials = null,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            bool zapBackingFields = false,
            Action<HttpWebRequest> customizeRequest = null,
            Action<HttpResponseMetadata, Stream> handleResponse = null
        )
        {
            var e = content != null
                ? await PostJsonAsync<E>(serviceUrl, content, method: "DELETE", securityProtocol: securityProtocol, headers: headers, credentials: credentials, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse)
                : await GetJsonAsync<E>(serviceUrl, method: "DELETE", headers: headers, securityProtocol: securityProtocol, certificatePath: certificatePath, certificatePfxPath: certificatePfxPath, certificatePfxPassword: certificatePfxPassword, certificateX509KeyStorageFlags: certificateX509KeyStorageFlags, proxyAddress: proxyAddress, proxyPort: proxyPort, proxyBypassOnLocal: proxyBypassOnLocal, proxyBypassList: proxyBypassList, proxyCredentials: proxyCredentials, zapBackingFields: zapBackingFields, customizeRequest: customizeRequest, handleResponse: handleResponse);
            return e;
        }

        private static HttpWebRequest GetWebRequest(string serviceUrl, string method, SecurityProtocolType securityProtocol)
        {
            var _securityProtocol = ServicePointManager.SecurityProtocol;
            if (serviceUrl.ToLower().StartsWith("https://"))
            {
                ServicePointManager.SecurityProtocol = securityProtocol;
            }
            var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
            ServicePointManager.SecurityProtocol = _securityProtocol;
            request.Method = method;
            return request;
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
