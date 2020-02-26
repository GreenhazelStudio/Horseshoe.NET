using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.Ftp
{
    // ref: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/how-to-upload-files-with-ftp
    public static class Ftp
    {
        public static event Action<string> RequestUriCreated;
        public static event Action<string, long, int, string> FileUploaded;
        public static event Action<string, long, int, string> FileDownloaded;
        public static event Action<int, int, string> DirectoryContentsListed;

        public static void UploadFile
        (
            string filePath,
            string newFileName = null,
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

            UploadContent(fileContents, newFileName ?? Path.GetFileName(filePath), server: server, port: port, serverPath: serverPath, credentials: credentials);
        }

        public static void UploadFile
        (
            FileInfo file,
            string newFileName = null,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            UploadFile
            (
                file.FullName,
                newFileName: newFileName,
                server: server,
                port: port,
                serverPath: serverPath,
                credentials: credentials,
                isBinary: isBinary,
                encoding: encoding
            );
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
            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server ?? FtpSettings.DefaultFtpServer, port ?? FtpSettings.DefaultPort, serverPath, fileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = (credentials ?? FtpSettings.DefaultCredentials)?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                FileUploaded?.Invoke(TextUtil.RevealNullOrBlank(fileName), bytes.LongLength, (int)response.StatusCode, response.StatusDescription);
            }
        }

        public static void DownloadFile
        (
            string serverFileName,
            string downloadFilePath,
            bool overwrite = false,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null
        )
        {
            var stream = DownloadFile
            (
                serverFileName,
                server: server,
                port: port,
                serverPath: serverPath,
                credentials: credentials
            );
            if (Directory.Exists(downloadFilePath))
            {
                downloadFilePath = Path.Combine(downloadFilePath, serverFileName);
            }
            if (File.Exists(downloadFilePath) && !overwrite)
            {
                throw new UtilityException("A file named '" + Path.GetFileName(downloadFilePath) + "' already exists in '" + Path.GetDirectoryName(downloadFilePath) + "'");
            }
            File.WriteAllBytes(downloadFilePath, stream.ToArray());
        }

        public static MemoryStream DownloadFile
        (
            string serverFileName,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null
        )
        {
            var memoryStream = new MemoryStream();

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server ?? FtpSettings.DefaultFtpServer, port ?? FtpSettings.DefaultPort, serverPath, serverFileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = (credentials ?? FtpSettings.DefaultCredentials)?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                var stream = response.GetResponseStream();
                stream.CopyTo(memoryStream);
                FileDownloaded?.Invoke(serverFileName, memoryStream.Length, (int)response.StatusCode, response.StatusDescription);
            }

            return memoryStream;
        }

        public static string[] ListDetailedDirectoryContents
        (
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null
        )
        {
            string[] contents;

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server ?? FtpSettings.DefaultFtpServer, port ?? FtpSettings.DefaultPort, serverPath, null);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = (credentials ?? FtpSettings.DefaultCredentials)?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                var streamReader = new StreamReader(response.GetResponseStream());
                contents = streamReader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
                DirectoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
            }

            return contents;
        }


        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            string server = null,
            int? port = null,
            string serverPath = "/",
            Credential? credentials = null
        )
        {
            string[] contents;
            Regex filter = null;

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server ?? FtpSettings.DefaultFtpServer, port ?? FtpSettings.DefaultPort, serverPath, null);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = (credentials ?? FtpSettings.DefaultCredentials)?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            // Convert the file mask to regex
            if (fileMask == FtpFileMasks.NoExtension)
            {
                filter = new Regex("^[^.]+$");
            }
            else if (fileMask != null)
            {
                filter = new Regex(fileMask.Replace(".", @"\.").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                var streamReader = new StreamReader(response.GetResponseStream());
                contents = streamReader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
                if (filter != null)
                {
                    contents = contents
                        .Where(s => filter.IsMatch(s))
                        .ToArray();
                }
                DirectoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
            }

            return contents;
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
