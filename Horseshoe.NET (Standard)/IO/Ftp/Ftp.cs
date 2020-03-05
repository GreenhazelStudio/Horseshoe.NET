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
        public static event Action<string, int, string> FileDeleted;

        public static void UploadFile
        (
            string filePath,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/",
            string serverFileName = null,
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

            UploadFile(serverFileName ?? Path.GetFileName(filePath), fileContents, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            FileInfo file,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/",
            string serverFileName = null,
            bool isBinary = false,
            Encoding encoding = null
        )
        {
            UploadFile
            (
                file.FullName,
                connectionInfo: connectionInfo,
                serverPath: serverPath,
                serverFileName: serverFileName,
                isBinary: isBinary,
                encoding: encoding
            );
        }

        public static void UploadFile
        (
            string fileName,
            string contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/",
            Encoding encoding = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(contents);

            UploadFile(fileName, fileContents, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            string fileName,
            byte[] contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            string server;
            int? port;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                serverPath = connectionInfo.ServerPath ?? serverPath;
                credentials = connectionInfo.Credentials;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                credentials = FtpSettings.DefaultCredentials;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, serverPath, fileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");
            request.ContentLength = contents.LongLength;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(contents, 0, contents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                FileUploaded?.Invoke(TextUtil.RevealNullOrBlank(fileName), contents.LongLength, (int)response.StatusCode, response.StatusDescription);
            }
        }

        public static void DownloadFile
        (
            string serverFileName,
            string downloadFilePath,
            bool overwrite = false,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            var stream = DownloadFile
            (
                serverFileName,
                connectionInfo: connectionInfo,
                serverPath: serverPath
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
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            var memoryStream = new MemoryStream();
            string server;
            int? port;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                serverPath = connectionInfo.ServerPath ?? serverPath;
                credentials = connectionInfo.Credentials;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                credentials = FtpSettings.DefaultCredentials;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, serverPath, serverFileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                var stream = response.GetResponseStream();
                stream.CopyTo(memoryStream);
                FileDownloaded?.Invoke(serverFileName, memoryStream.Length, (int)response.StatusCode, response.StatusDescription);
            }

            return memoryStream;
        }

        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            string[] contents;
            Regex filter = null;
            string server;
            int? port;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                serverPath = connectionInfo.ServerPath ?? serverPath;
                credentials = connectionInfo.Credentials;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                credentials = FtpSettings.DefaultCredentials;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, serverPath, null);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

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

        public static string[] ListDetailedDirectoryContents
        (
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            string[] contents;
            string server;
            int? port;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                serverPath = connectionInfo.ServerPath ?? serverPath;
                credentials = connectionInfo.Credentials;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                credentials = FtpSettings.DefaultCredentials;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, serverPath, null);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                var streamReader = new StreamReader(response.GetResponseStream());
                contents = streamReader.ReadToEnd().Replace("\r\n", "\n").Split('\n');
                DirectoryContentsListed?.Invoke(contents.Length, (int)response.StatusCode, response.StatusDescription);
            }

            return contents;
        }

        public static void DeleteFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = "/"
        )
        {
            string server;
            int? port;
            Credential? credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port;
                serverPath = connectionInfo.ServerPath ?? serverPath;
                credentials = connectionInfo.Credentials;
            }
            else
            {
                server = FtpSettings.DefaultFtpServer;
                port = FtpSettings.DefaultPort;
                credentials = FtpSettings.DefaultCredentials;
            }

            // Get the object used to communicate with the server
            var uriString = CreateRequestUriString(server, port, serverPath, serverFileName);
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = credentials?.ToNetworkCredentials() ?? new NetworkCredential("anonymous", "ftpuser");

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                FileDeleted?.Invoke(serverFileName, (int)response.StatusCode, response.StatusDescription);
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
