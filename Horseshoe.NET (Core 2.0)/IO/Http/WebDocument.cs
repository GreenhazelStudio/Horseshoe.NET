using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.Http
{
    public static class WebDocument
    {
        public static Stream GetStream
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = null;
            WebService.Get
            (
                documentURL,
                method: "GET",
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest,
                handleResponse: (metadata, _stream) => { stream = _stream; metadata.KeepStreamOpen = true; }
            );
            return stream;
        }

        public static byte[] GetBytes
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = GetStream
            (
                documentURL,
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest
            );
            var bytes = new List<byte>();
            var buf = new byte[10240];
            using (stream)
            {
                while (true)
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

        public static bool Download
        (
            string documentURL,
            string filePath,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = GetStream
            (
                documentURL,
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest
            );
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                using (stream)
                {
                    stream.CopyTo(writer, 10240);
                }
            }
            return true;
        }

        public static string GetText
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
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
            var text = WebService.Get
            (
                documentURL,
                method: "GET",
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest,
                handleResponse: handleResponse
            );
            return text;
        }

        public static async Task<Stream> GetStreamAsync
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = null;
            await WebService.GetAsync
            (
                documentURL,
                method: "GET",
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest,
                handleResponse: (metadata, _stream) => { stream = _stream; metadata.KeepStreamOpen = true; }
            );
            return stream;
        }


        public static async Task<byte[]> GetBytesAsync
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = await GetStreamAsync
            (
                documentURL,
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest
            );
            var bytes = new List<byte>();
            var buf = new byte[10240];
            using (stream)
            {
                while (true)
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

        public static async Task<bool> DownloadAsync
        (
            string documentURL,
            string filePath,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
            string certificatePath = null,
            string certificatePfxPath = null,
            string certificatePfxPassword = null,
            X509KeyStorageFlags? certificateX509KeyStorageFlags = null,
            string proxyAddress = null,
            int? proxyPort = null,
            bool proxyBypassOnLocal = false,
            string[] proxyBypassList = null,
            Credential? proxyCredentials = null,
            Action<HttpWebRequest> customizeRequest = null
        )
        {
            Stream stream = await GetStreamAsync
            (
                documentURL,
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest
            );
            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                using (stream)
                {
                    await stream.CopyToAsync(writer, 10240);
                }
            }
            return true;
        }

        public static async Task<string> GetTextAsync
        (
            string documentURL,
            IDictionary<object, string> headers = null,
            Credential? credentials = null,
            SecurityProtocolType securityProtocol = SecurityProtocolType.SystemDefault,
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
            var text = await WebService.GetAsync
            (
                documentURL,
                method: "GET",
                headers: headers,
                credentials: credentials,
                securityProtocol: securityProtocol,
                certificatePath: certificatePath,
                certificatePfxPath: certificatePfxPath,
                certificatePfxPassword: certificatePfxPassword,
                certificateX509KeyStorageFlags: certificateX509KeyStorageFlags,
                proxyAddress: proxyAddress,
                proxyPort: proxyPort,
                proxyBypassOnLocal: proxyBypassOnLocal,
                proxyBypassList: proxyBypassList,
                proxyCredentials: proxyCredentials,
                customizeRequest: customizeRequest,
                handleResponse: handleResponse
            );
            return text;
        }
    }
}
