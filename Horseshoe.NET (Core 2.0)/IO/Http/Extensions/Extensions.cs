using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;

namespace Horseshoe.NET.IO.Http.Extensions
{
    public static class Extensions
    {
        public static IDictionary<string, string[]> ToOwinDictionary(this WebHeaderCollection collection)
        {
            if (collection == null) return null;
            var dict = new Dictionary<string, string[]>();
            for (int i = 0; i < collection.Count; i++) 
            {
                var name = collection.GetKey(i);
                var values = collection.GetValues(name);
                if (!values.Any()) continue;
                dict.Add(name, values);
            }
            return dict;
        }

        internal static void ProcessHeaders(this HttpWebRequest request, IDictionary<object, string> headers, string contentType = null)
        {
            if (headers != null)
            {
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

            if (contentType != null)
            {
                request.ContentType = contentType;
            }
        }

        internal static void ProcessContent(this HttpWebRequest request, object content, Func<object, string> contentSerializer)
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

        internal static void ProcessCredentials(this HttpWebRequest request, Credential? credentials)
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

        internal static void ProcessCertificates(this HttpWebRequest request, string certificatePath, string certificatePfxPath, string certificatePfxPassword, X509KeyStorageFlags? certificateX509KeyStorageFlags)
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

        internal static string ProcessResponse(this HttpWebResponse response, Action<HttpResponseMetadata, Stream> handleResponse)
        {
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

        internal static async Task<string> ProcessResponseAsync(this HttpWebResponse response, Action<HttpResponseMetadata, Stream> handleResponse)
        {
            var responseStream = response.GetResponseStream();
            if (handleResponse != null)
            {
                var responseMetadata = new HttpResponseMetadata(response.StatusCode, response.StatusDescription, response.Headers.ToOwinDictionary());
                handleResponse.Invoke(responseMetadata, responseStream);
                responseStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(responseStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        internal static NetworkCredential ToNetworkCredential(this Credential credentials, CryptoOptions options = null)
        {
            if (credentials.HasSecurePassword)
            {
                return new NetworkCredential(credentials.UserName, credentials.SecurePassword);
            }

            if (credentials.Password != null)
            {
                SecureString securePassword;
                if (credentials.IsEncryptedPassword)
                {
                    securePassword = Decrypt.SecureString(credentials.Password, options: options);
                }
                else
                {
                    securePassword = new SecureString();
                    foreach (char c in credentials.Password)
                    {
                        securePassword.AppendChar(c);
                    }
                    securePassword.MakeReadOnly();
                }
                return new NetworkCredential(credentials.UserName, securePassword);
            }
            return new NetworkCredential(credentials.UserName, credentials.Password);
        }

        internal static void ProcessProxy(this HttpWebRequest request, string proxyAddress, int? proxyPort, bool proxyBypassOnLocal, string[] proxyBypassList, Credential? proxyCredentials)
        {
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                if (proxyCredentials is Credential creds)
                {
                    request.Proxy = new WebProxy(proxyAddress + (proxyPort.HasValue ? ":" + proxyPort : ""), proxyBypassOnLocal, proxyBypassList, creds.ToNetworkCredential());
                }
                else if(proxyBypassList != null)
                {
                    request.Proxy = new WebProxy(proxyAddress + (proxyPort.HasValue ? ":" + proxyPort : ""), proxyBypassOnLocal, proxyBypassList);
                }
                else if (proxyPort.HasValue)
                {
                    request.Proxy = new WebProxy(proxyAddress, proxyPort.Value);
                }
                else
                {
                    request.Proxy = new WebProxy(proxyAddress, proxyBypassOnLocal);
                }
            }
        }
    }
}
