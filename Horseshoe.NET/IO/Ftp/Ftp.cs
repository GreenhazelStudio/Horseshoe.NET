using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Ftp
{
    // ref: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
    public static class Ftp
    {
        public static event Action<string> RequestUriCreated;
        public static event Action<string, long, int, string> FileUploaded;

        public static void UploadFile
        (
            string filePath,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            byte[] fileContents;

            // Get the file bytes
            if (isBinary)
            {
                fileContents = File.ReadAllBytes(filePath);
            }
            else
            {
                using (StreamReader sourceStream = new StreamReader(filePath))
                {
                    fileContents = (encoding ?? Encoding.Default).GetBytes(sourceStream.ReadToEnd());
                }
            }

            UploadContent(fileContents, Path.GetFileName(filePath), server: server, port: port, serverPath: serverPath, credentials: credentials);
        }

        public static void UploadFile
        (
            FileInfo file,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            byte[] fileContents;

            // Get the file bytes
            if (isBinary)
            {
                fileContents = File.ReadAllBytes(file.FullName);
            }
            else
            {
                using (var streamReader = new StreamReader(file.OpenRead()))
                {
                    fileContents = (encoding ?? Encoding.Default).GetBytes(streamReader.ReadToEnd());
                }
            }

            UploadContent(fileContents, file.Name, server: server, port: port, serverPath: serverPath, credentials: credentials);
        }

        public static void UploadContent
        (
            string content,
            string fileName,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null,
            Encoding encoding = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(content);

            UploadContent(fileContents, fileName, server: server, port: port, serverPath: serverPath, credentials: credentials);
        }

        public static void UploadContent
        (
            byte[] bytes,
            string fileName,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null
        )
        {
            // Get the object used to communicate with the server.
            var uriString = CreateRequestUriString(server ?? Settings.DefaultFtpServer, port ?? Settings.DefaultPort, serverPath, fileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                FileUploaded?.Invoke(fileName ?? "[null]", bytes.LongLength, (int)response.StatusCode, response.StatusDescription);
            }
        }

        static string CreateRequestUriString(string server, int? port, string serverPath, string fileName)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            var sb = new StringBuilder()
                .AppendIf
                (
                    !server.Contains("://"),
                    "ftp://"
                )
                .AppendIf
                (
                    server.EndsWith("/"),
                    server.Substring(0, server.Length - 1),
                    server
                )
                .AppendIf
                (
                    port.HasValue,
                    ":" + port
                )
                .Append("/");
            if (serverPath != null)
            {
                sb  .AppendIf
                    (
                        serverPath.StartsWith("/"),
                        serverPath.Substring(1),
                        serverPath
                    )
                    .AppendIf
                    (
                        !serverPath.EndsWith("/"),
                        "/"
                    );
            }
            if (fileName != null)
            {
                sb.Append(fileName);
            }
            RequestUriCreated?.Invoke(sb.ToString());
            return sb.ToString();
        }
    }
}
