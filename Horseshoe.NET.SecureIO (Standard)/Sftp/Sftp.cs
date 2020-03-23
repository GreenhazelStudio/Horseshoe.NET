using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.Crypto;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.Ftp;
using Horseshoe.NET.Text;

using Renci.SshNet;

namespace Horseshoe.NET.SecureIO.Sftp
{
    // ref: https://www.codeproject.com/Tips/1111060/Upload-File-to-SFTP-Site-with-Csharp-in-Visual-Stu
    public static class Sftp
    {
        public static event FileUploaded FileUploaded;
        public static event FileDownloaded FileDownloaded;
        public static event DirectoryContentsListed DirectoryContentsListed;
        public static event FileDeleted FileDeleted;

        public static void UploadFile
        (
            string filePath,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null,
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
            string serverPath = null,
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
            string serverPath = null,
            Encoding encoding = null
        )
        {
            // Get the content bytes
            byte[] fileContents = (encoding ?? Encoding.Default).GetBytes(contents);

            UploadFile(fileName, fileContents, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            NamedMemoryStream namedStream,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            UploadFile(namedStream.Name, namedStream, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            string fileName,
            byte[] contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            var memoryStream = new MemoryStream();
            memoryStream.Write(contents, 0, contents.Length);
            UploadFile(fileName, memoryStream, connectionInfo: connectionInfo, serverPath: serverPath);
        }

        public static void UploadFile
        (
            string fileName,
            Stream contents,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    long bytesUploaded = 0L;
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.BufferSize = 4 * 1024;
                    server.UploadFile(contents, fileName, (ulongValue) => bytesUploaded = (long)ulongValue);
                    FileUploaded?.Invoke(TextUtil.RevealNullOrBlank(fileName), bytesUploaded, 0, "File upload action raised no errors");
                }
            );
        }

        public static void DownloadFile
        (
            string serverFileName,
            string downloadFilePath,
            bool overwrite = false,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
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

        public static NamedMemoryStream DownloadFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            var memoryStream = new NamedMemoryStream(serverFileName);

            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.DownloadFile(serverFileName, memoryStream);
                    FileDownloaded?.Invoke(serverFileName, memoryStream.Length, 0, "File download action raised no errors");
                }
            );

            return memoryStream;
        }

        public static string[] ListDirectoryContents
        (
            string fileMask = null,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            string[] contents = null;

            // Convert the file mask to regex
            Regex filter = null;
            if (fileMask == FtpFileMasks.NoExtension)
            {
                filter = new Regex("^[^.]+$");
            }
            else if (fileMask != null)
            {
                filter = new Regex(fileMask.Replace(".", @"\.").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
            }

            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    var files = server.ListDirectory(resultantServerPath);
                    contents = files.Select(f => f.Name).ToArray();
                    if (filter != null)
                    {
                        contents = contents
                            .Where(s => filter.IsMatch(s))
                            .ToArray();
                    }
                    DirectoryContentsListed?.Invoke(contents.Length, 0, "List directory action raised no errors");
                }
            );

            return contents;
        }

        public static void DeleteFile
        (
            string serverFileName,
            FtpConnectionInfo connectionInfo = null,
            string serverPath = null
        )
        {
            ConnectAndDoAction
            (
                connectionInfo,
                serverPath,
                action: (server, resultantServerPath) =>
                {
                    if (resultantServerPath.Length > 0)
                    {
                        server.ChangeDirectory(resultantServerPath);
                    }
                    server.DeleteFile(serverFileName);
                    FileDeleted?.Invoke(serverFileName, 0, "Delete action raised no errors");
                }
            );
        }

        static void ConnectAndDoAction
        (
            FtpConnectionInfo connectionInfo,
            string serverPath,
            Action<SftpClient, string> action    // server, resultantServerPath
        )
        {
            string server;
            int port;
            Credential credentials;

            if (connectionInfo != null)
            {
                server = connectionInfo.Server;
                port = connectionInfo.Port ?? 22;
                credentials = connectionInfo.Credentials ?? new Credential("anonymous", "ftpuser");
                serverPath = connectionInfo.ServerPath ?? serverPath;
            }
            else
            {
                server = SftpSettings.DefaultFtpServer;
                port = SftpSettings.DefaultPort ?? 22;
                credentials = SftpSettings.DefaultCredentials ?? new Credential("anonymous", "ftpuser");
                serverPath = serverPath ?? SftpSettings.DefaultServerPath;
            }

            string password;
            if (credentials.HasSecurePassword)
            {
                password = credentials.SecurePassword.ToUnsecureString();
            }
            else if (credentials.IsEncryptedPassword)
            {
                password = Decrypt.String(credentials.Password);
            }
            else
            {
                password = credentials.Password;
            }

            if (serverPath.StartsWith("/"))
            {
                serverPath = serverPath.Substring(1);
            }

            // Get the object used to communicate with the server
            using (var client = new SftpClient(server, port, credentials.UserName, password))
            {
                client.Connect();
                action.Invoke(client, serverPath);
            }
        }
    }
}
