using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.Ftp;
using Horseshoe.NET.IO.ReportingServices;
using Horseshoe.NET.SecureIO.Sftp;

namespace TestConsole
{
    class IOTests : Routine
    {
        public override Title Title => "IO Tests";
        public override bool Looping => true;

        static IOTests()
        {
            Ftp.RequestUriCreated += (uri) => Console.WriteLine("URI: " + uri);
            Ftp.FileUploaded += (fileName, fileSize, statusCode, statusDescription) => Console.WriteLine("Upload results: " + fileName + " - " + FileUtil.GetDisplayFileSize(fileSize) + " - " + statusDescription);
            Ftp.FileDeleted += (fileName, statusCode, statusDescription) => Console.WriteLine("Delete results: " + fileName + " - " + statusDescription);
            Ftp.DirectoryContentsListed += (count, statusCode, statusDescription) => Console.WriteLine("Dir listing results: x" + count + " - " + statusDescription);
            Sftp.FileUploaded += (fileName, fileSize, statusCode, statusDescription) => Console.WriteLine("Upload results: " + fileName + " - " + FileUtil.GetDisplayFileSize(fileSize) + " - " + statusDescription);
            Sftp.FileDeleted += (fileName, statusCode, statusDescription) => Console.WriteLine("Delete results: " + fileName + " - " + statusDescription);
            Sftp.DirectoryContentsListed += (count, statusCode, statusDescription) => Console.WriteLine("Dir listing results: x" + count + " - " + statusDescription);
        }

        public override void Do()
        {
            var selection = PromptMenu
            (
                new[]
                {
                    "Build SSRS URLs",
                    "Display file sizes",
                    "Test FTP Upload",
                    "Test SFTP Upload",
                    "Test FTP Download",
                    "Test SFTP Download",
                    "Test FTP Delete",
                    "Test SFTP Delete",
                    "List FTP Directory",
                    "List SFTP Directory",
                    "regex",
                    "FTP connection string parse tests"
                },
                title: "SSRS Test Menu"
            );
            var ftpPseudoConnectionString = "ftp://nexusftp@192.168.155.130//u02/app/se4prod/del4/work/edidir/flat_in?password=nexusftp";
            var sftpPseudoConnectionString = "sftp://nexusftp@192.168.155.130//u02/app/se4prod/del4/work/edidir/flat_in?password=nexusftp";

            switch (selection.SelectedItem)
            {
                case "Build SSRS URLs":
                    Console.WriteLine("File: " + ReportUtil.BuildFileUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    Console.WriteLine("Link: " + ReportUtil.BuildHyperlinkUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    break;
                case "Display file sizes":
                    Console.WriteLine("-1 B  =>  " + FileUtil.GetDisplayFileSize(-1));
                    Console.WriteLine("-1 B in KB  =>  " + FileUtil.GetDisplayFileSize(-1, unit: FileSize.Unit.KB));
                    Console.WriteLine("0  =>  " + FileUtil.GetDisplayFileSize(0));
                    Console.WriteLine("0 B in GB  =>  " + FileUtil.GetDisplayFileSize(0, unit: FileSize.Unit.GB));
                    Console.WriteLine("1000000  =>  " + FileUtil.GetDisplayFileSize(1000000));
                    Console.WriteLine("1000000 'bi'  =>  " + FileUtil.GetDisplayFileSize(1000000, bi: true));
                    Console.WriteLine("1000000 in B  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSize.Unit.B));
                    Console.WriteLine("1000000 in B w/o sep  =>  " + FileUtil.GetDisplayFileSize(1000000, addSeparators: false, unit: FileSize.Unit.B));
                    Console.WriteLine("1000000 B in KB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSize.Unit.KB));
                    Console.WriteLine("1000000 B in KiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSize.Unit.KiB));
                    Console.WriteLine("1000000 B in GB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSize.Unit.GB));
                    Console.WriteLine("1000000 B in GiB  =>  " + FileUtil.GetDisplayFileSize(1000000, unit: FileSize.Unit.GiB));
                    Console.WriteLine("1000000 B in GB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSize.Unit.GB));
                    Console.WriteLine("1000000 B in GiB w/ 3 dec  =>  " + FileUtil.GetDisplayFileSize(1000000, maxDecimalPlaces: 3, unit: FileSize.Unit.GiB));
                    Console.WriteLine();
                    break;
                case "Test FTP Upload":
                    Ftp.UploadFile
                    (
                        "hello.txt",
                        "Hello World!",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    break;
                case "Test SFTP Upload":
                    Sftp.UploadFile
                    (
                        "hello.txt",
                        "Hello World!",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    break;
                case "Test FTP Download":
                    var stream = Ftp.DownloadFile
                    (
                        "hello.txt",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("File length: " + stream.Length);
                    Console.WriteLine("File contents: " + Encoding.Default.GetString(stream.ToArray()));
                    break;
                case "Test SFTP Download":
                    var sstream = Sftp.DownloadFile
                    (
                        "hello.txt",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("File length: " + sstream.Length);
                    Console.WriteLine("File contents: " + Encoding.Default.GetString(sstream.ToArray()));
                    break;
                case "Test FTP Delete":
                    Ftp.DeleteFile
                    (
                        "hello.txt",
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    break;
                case "Test SFTP Delete":
                    Sftp.DeleteFile
                    (
                        "hello.txt",
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    break;
                case "List FTP Directory":
                    var dirContents = Ftp.ListDirectoryContents
                    (
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents:");
                    Console.WriteLine(dirContents.Any() ? string.Join(Environment.NewLine, dirContents) : "[0 results]");
                    Console.WriteLine();

                    dirContents = Ftp.ListDirectoryContents
                    (
                        fileMask: FtpFileMasks.Txt,
                        connectionInfo: FtpUtil.ParseFtpConnectionString(ftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents (.txt files only):");
                    Console.WriteLine(dirContents.Any() ? string.Join(Environment.NewLine, dirContents) : "[0 results]");
                    break;
                case "List SFTP Directory":
                    var sdirContents = Sftp.ListDirectoryContents
                    (
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents:");
                    Console.WriteLine(string.Join(Environment.NewLine, sdirContents));
                    Console.WriteLine();

                    dirContents = Sftp.ListDirectoryContents
                    (
                        fileMask: FtpFileMasks.Txt,
                        connectionInfo: SftpUtil.ParseSftpConnectionString(sftpPseudoConnectionString)
                    );
                    Console.WriteLine("Directory contents (.txt files only):");
                    Console.WriteLine(string.Join(Environment.NewLine, dirContents));
                    break;
                case "regex":
                    var regexes = new[] { "^[^.]+$", "^[^\\.]+$" };
                    var testStrings = new[] { "file.txt", "DIR" };
                    foreach (var regex in regexes)
                    {
                        foreach (var str in testStrings)
                        {
                            Console.WriteLine("Testing '" + str + "' against '" + regex + "' -> " + Regex.IsMatch(str, regex));
                        }
                    }
                    break;
                case "FTP connection string parse tests":
                    var connStrs = new[]
                    {
                        "ftp://george@11.22.33.44/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                        "ftp://george@11.22.33.44:9001/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                        "george@11.22.33.44/dir/subdir?encryptedPassword=akdj$8iO(d@1sd==",
                    };
                    foreach(var str in connStrs)
                    {
                        Console.WriteLine("Imported:  " + str);
                        Console.Write("Exporting: ");
                        try
                        {
                            var connectionInfo = FtpUtil.ParseFtpConnectionString(str);
                            Console.WriteLine(connectionInfo);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("error: " + ex.Message);
                        }
                        Console.WriteLine();
                    }
                    break;
                
            }
        }
    }
}
