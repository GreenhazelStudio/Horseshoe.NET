using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Horseshoe.NET.Numbers;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO
{
    public static class FileUtil
    {
        private static Regex FullPathIndicator { get; } = new Regex(@"^([A-Z]:\\|\\\\|(http[s]?|ftp[s]?|file):\/\/).*", RegexOptions.IgnoreCase);

        public static bool IsFullPath(string path)
        {
            return FullPathIndicator.IsMatch(path);
        }

        public static string Create(byte[] contents, string targetDirectory = null, string targetFileName = null, FileType? fileType = null)
        {
            var targetPath = Path.Combine
            (
                targetDirectory ?? Path.GetTempPath(),
                targetFileName ?? "file" + (fileType.HasValue ? "." + fileType.ToString().ToLower() : "")
            );
            File.WriteAllBytes(targetPath, contents);
            return targetPath;
        }

        public static string Create(string contents, string targetDirectory = null, string targetFileName = null, FileType? fileType = null, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.Default).GetBytes(contents);
            return Create(bytes, targetDirectory: targetDirectory, targetFileName: targetFileName, fileType: fileType);
        }

        public static string DownloadFromWeb(string url, string targetDirectory = null, string targetFileName = null, FileType? fileType = null, Credential? credentials = null)
        {
            var targetPath = Path.Combine
            (
                targetDirectory ?? Path.GetTempPath(),
                targetFileName ?? TextUtil.Zap(ParseFileNameFromURL(url, "file", fileType))
            );
            var bytes = BytesFromWeb(url, credentials: credentials);
            File.WriteAllBytes(targetPath, bytes);
            return targetPath;
        }

        public static byte[] BytesFromWeb(string url, Credential? credentials = null)
        {
            using (var stream = new MemoryStream())
            {
                var responseStream = StreamFromWeb(url, credentials: credentials);
                responseStream.CopyTo(stream);
                responseStream.Flush();
                responseStream.Close();
                return stream.ToArray();
            }
        }

        public static Stream StreamFromWeb(string url, Credential? credentials = null)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.Credentials = credentials.HasValue
                ? new NetworkCredential(credentials.Value.UserID, credentials.Value.UserID)
                : CredentialCache.DefaultCredentials;
            request.Timeout = 600000;
            var response = request.GetResponse() as HttpWebResponse;
            return response.GetResponseStream();
        }

        public static string ParseFileNameFromURL(string url, string nameIfBlank, FileType? fileType)
        {
            return ParseFileNameFromURL(url, nameIfBlank, fileType?.ToString().ToLower());
        }

        public static string ParseFileNameFromURL(string url, string nameIfBlank, string extension)
        {
            if (url == null) return null;
            if (url.Contains(":///"))                             // file:///share/dir/subdir/file.txt
            {
                url = url.Substring(url.IndexOf(":///") + 4);     // -> share/dir/subdir/file.txt
            }
            else if (url.Contains("://"))                         // http://example.com/FileSelect.aspx?filepath=dir/subdir/file.txt
            {
                url = url.Substring(url.IndexOf("://") + 3);      // -> example.com/FileSelect.aspx?filepath=dir/subdir/file.txt
            }
            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf("?"));         // -> example.com/FileSelect.aspx
            }
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);           // example.com/apps/FileSelect/ -> example.com/apps/FileSelect
            }
            if (url.Contains("/"))
            {
                url = url.Substring(url.LastIndexOf("/") + 1);    // example.com/FileSelect.aspx -> FileSelect.aspx
            }
            if (url.Length == 0)
            {
                if (string.IsNullOrWhiteSpace(nameIfBlank)) return "";
                url = nameIfBlank;
            }
            if (url.Length > 0 && !string.IsNullOrWhiteSpace(extension))
            {
                var loext = extension.ToLower();
                var lourl = url.ToLower();
                switch (loext)
                {
                    case "jpg":
                    case "jpeg":
                        if (!(lourl.EndsWith(".jpg") || lourl.EndsWith(".jpeg")))
                        {
                            url += "." + loext;
                        }
                        break;
                    case "mpg":
                    case "mpeg":
                        if (!(lourl.EndsWith(".mpg") || lourl.EndsWith(".mpeg")))
                        {
                            url += "." + loext;
                        }
                        break;
                    case "tif":
                    case "tiff":
                        if (!(lourl.EndsWith(".tif") || lourl.EndsWith(".tiff")))
                        {
                            url += "." + loext;
                        }
                        break;
                    default:
                        if (!lourl.EndsWith("." + loext))
                        {
                            url += "." + loext;
                        }
                        break;
                }
            }
            return url;
        }

        public static string AppendExtensionIf(string fileName, FileType extension, bool lowerCase = false)
        {
            var extensionText = "." + extension;
            if (lowerCase)
            {
                extensionText = extensionText.ToLower();
            }
            return AppendExtensionIf(fileName, extensionText);
        }

        public static string AppendExtensionIf(string fileName, string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            return TextUtil.AppendIf(fileName, extension, !fileName.ToLower().EndsWith(extension.ToLower()));
        }

        /// <summary>
        /// Moves a directory to a new path.  Imitates PowerShell Move-Item cmdlet behavior (i.e. if dest folder exists dest folder becomes the parent folder)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="overwriteFiles"></param>
        /// <param name="overwriteFileStructure"></param>
        public static void MoveDirectory(string sourcePath, string targetPath, bool overwriteFiles = true, bool overwriteFileStructure = false)
        {
            CopyDirectory(sourcePath, targetPath, mode: DirectoryCopyMode.Move, overwriteFiles: overwriteFiles, overwriteFileStructure: overwriteFileStructure);
        }

        /// <summary>
        /// Copies or merges a directory to a new path.  Imitates PowerShell Copy-Item cmdlet behavior (i.e. if dest folder exists dest folder becomes the parent folder)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <param name="mode"></param>
        /// <param name="overwriteFiles"></param>
        /// <param name="overwriteFileStructure"></param>
        public static void CopyDirectory(string sourcePath, string targetPath, DirectoryCopyMode mode = DirectoryCopyMode.Copy, bool overwriteFiles = true, bool overwriteFileStructure = false)
        {
            var sourceDir = new DirectoryInfo(sourcePath.EndsWith(@"\") ? sourcePath.Substring(0, sourcePath.Length - 1) : sourcePath);
            var targetDir = new DirectoryInfo(targetPath.EndsWith(@"\") ? targetPath.Substring(0, targetPath.Length - 1) : targetPath);
            if (!sourceDir.Exists)
            {
                throw new UtilityException("Source directory not found: " + sourcePath);
            }
            if (File.Exists(targetDir.FullName))
            {
                throw new UtilityException("Target file already exists: " + targetPath);
            }
            if (targetDir.Exists)
            {
                targetDir = new DirectoryInfo(Path.Combine(targetPath, sourceDir.Name));
                if (File.Exists(targetDir.FullName))
                {
                    throw new UtilityException("Target file already exists: " + targetPath);
                }
            }
            if (sourceDir.FullName.Equals(targetDir.FullName, StringComparison.OrdinalIgnoreCase))
            {
                throw new UtilityException("Source and target may not be the same directory");
            }
            if (targetDir.Exists && overwriteFileStructure)
            {
                targetDir.Delete(true);
                Thread.Sleep(10);  // just in case Directory.Exists returns a false positive if queried immediately after a delete
            }
            _CopyDirectory(sourceDir.FullName, targetDir.FullName, sourceDir, mode, overwriteFiles);
        }

        private static void _CopyDirectory(string sourceHome, string targetHome, DirectoryInfo sourceDir, DirectoryCopyMode mode, bool overwriteFiles)
        {
            DirectoryInfo targetDir;
            if (sourceHome.Equals(sourceDir.FullName, StringComparison.OrdinalIgnoreCase))
            {
                targetDir = new DirectoryInfo(targetHome);
            }
            else
            {
                var sourcePathDelta = sourceDir.FullName.Substring(sourceHome.Length + 1);  // + 1 for slash
                targetDir = new DirectoryInfo(Path.Combine(targetHome, sourcePathDelta));
            }

            if (mode == DirectoryCopyMode.Move && sourceDir.Root.FullName.Equals(targetDir.Root.FullName, StringComparison.OrdinalIgnoreCase) && !targetDir.Exists)
            {
                try
                {
                    sourceDir.MoveTo(targetDir.FullName);  // this usually fails for some reason - note: even the Move-Item cmdlet in PowerShell usually fails
                    return;
                }
                catch
                {
                }
            }

            var files = sourceDir.GetFiles();
            var directories = sourceDir.GetDirectories();

            targetDir.Create();

            string targetFileName;
            foreach (var file in files)
            {
                targetFileName = Path.Combine(targetDir.FullName, file.Name);
                if (mode == DirectoryCopyMode.Move && sourceDir.Root.FullName.Equals(targetDir.Root.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        file.MoveTo(targetFileName);  // this usually fails for some reason
                        continue;
                    }
                    catch
                    {
                    }
                }
                file.CopyTo(targetFileName, overwriteFiles);
                if (mode == DirectoryCopyMode.Move)
                {
                    file.Delete();
                }
            }

            foreach (var dir in directories)
            {
                _CopyDirectory(sourceHome, targetHome, dir, mode, overwriteFiles);
            }

            if (mode == DirectoryCopyMode.Move)
            {
                sourceDir.Delete();
            }
        }

        public const long KiB = 1024L;
        public const long MiB = KiB * 1024L;
        public const long GiB = MiB * 1024L;
        public const long TiB = GiB * 1024L;
        public const long PiB = TiB * 1024L;
        public const long EiB = PiB * 1024L;

        public static string GetDisplayFileSize(FileInfo file, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSizeUnit? unitToUse = null, bool useShortAbbreviation = false)
        {
            return GetDisplayFileSize(file.Length, minDecimalPlaces: minDecimalPlaces, maxDecimalPlaces: maxDecimalPlaces, addSeparators: addSeparators, unit: unitToUse, useShortAbbreviation: useShortAbbreviation);
        }

        public static string GetDisplayFileSize(long size, int? minDecimalPlaces = null, int? maxDecimalPlaces = null, bool addSeparators = true, FileSizeUnit? unit = null, bool useShortAbbreviation = false)
        {
            minDecimalPlaces = minDecimalPlaces ?? 0;
            maxDecimalPlaces = maxDecimalPlaces ?? NumberFormatInfo.CurrentInfo.NumberDecimalDigits;

            if (minDecimalPlaces < 0)
            {
                throw new UtilityException("Minimum decimal places must be >= 0");
            }
            if (maxDecimalPlaces < minDecimalPlaces)
            {
                throw new UtilityException("Maximum decimal places must be >= minimum decimal places");
            }

            var groupSizes = NumberFormatInfo.CurrentInfo.NumberGroupSizes;
            var decimSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var groupSep = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;

            var sb = new StringBuilder();
            if (addSeparators)
            {
                sb.Append("#" + groupSep + "#".Repeat(groupSizes[0]));
            }
            else
            {
                sb.Append("#");
            }
            if (maxDecimalPlaces > 0)
            {
                sb.Append(decimSep);
                sb.Append("0".Repeat(minDecimalPlaces.Value));
                sb.Append("#".Repeat(maxDecimalPlaces.Value));
            }
            var format = sb.ToString();

            var unitToUse = unit;
            if (!unitToUse.HasValue)
            {
                if(size < KiB)
                {
                    unitToUse = FileSizeUnit.B;
                }
                else if (size < MiB)
                {
                    unitToUse = FileSizeUnit.KiB;
                }
                else if (size < GiB)
                {
                    unitToUse = FileSizeUnit.MiB;
                }
                else if (size < TiB)
                {
                    unitToUse = FileSizeUnit.GiB;
                }
                else if (size < PiB)
                {
                    unitToUse = FileSizeUnit.TiB;
                }
                else if (size < EiB)
                {
                    unitToUse = FileSizeUnit.PiB;
                }
                else
                {
                    unitToUse = FileSizeUnit.EiB;
                }
            }

            double result = 0;

            switch (unitToUse)
            {
                case FileSizeUnit.B:
                    result = size;
                    break;
                default:
                    long multiplier;
                    switch (unitToUse)
                    {
                        case FileSizeUnit.KiB:
                            multiplier = KiB;
                            break;
                        case FileSizeUnit.MiB:
                            multiplier = MiB;
                            break;
                        case FileSizeUnit.GiB:
                            multiplier = GiB;
                            break;
                        case FileSizeUnit.TiB:
                            multiplier = TiB;
                            break;
                        case FileSizeUnit.PiB:
                            multiplier = PiB;
                            break;
                        case FileSizeUnit.EiB:
                        default:
                            multiplier = EiB;
                            break;
                    }

                    // calculate result
                    while (size >= multiplier)
                    {
                        result += 1D;
                        size -= multiplier;
                    }
                    result = NumberUtil.Trunc(result + size / (double)multiplier, maxDecimalPlaces.Value);
                    break;
            }

            var completeFormat = "{0:" + format + " \"" + (useShortAbbreviation ? unitToUse.ToString().Replace("i", "") : unitToUse.ToString()) + "\"}";
            return string.Format(completeFormat, result);
        }
    }
}
