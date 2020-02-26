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
            Ftp.DirectoryContentsListed += (count, statusCode, statusDescription) => Console.WriteLine("Dir listing results: x" + count + " - " + statusDescription);
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
                    "Test FTP Download",
                    "List FTP Directory",
                    "regex"
                },
                title: "SSRS Test Menu"
            );
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
                    Ftp.UploadContent
                    (
                        "Hello World!",
                        "hello.txt",
                        server: "11.22.33.44",
                        serverPath: "/my_dir",
                        credentials: new Credential("username", "password")
                    );
                    break;
                case "Test FTP Download":
                    var stream = Ftp.DownloadFile
                    (
                        "blank.txt",
                        server: "11.22.33.44",
                        serverPath: "/my_dir",
                        credentials: new Credential("username", "password")
                    );
                    Console.WriteLine("File length: " + stream.Length);
                    Console.WriteLine("File contents: " + Encoding.Default.GetString(stream.ToArray()));
                    break;
                case "List FTP Directory":
                    var dirContents = Ftp.ListDirectoryContents
                    (
                        server: "11.22.33.44",
                        serverPath: "/my_dir",
                        credentials: new Credential("username", "password")
                    );
                    Console.WriteLine("Directory contents:");
                    Console.WriteLine(string.Join(Environment.NewLine, dirContents));
                    Console.WriteLine();

                    dirContents = Ftp.ListDirectoryContents
                    (
                        fileMask: FtpFileMasks.Txt,
                        server: "11.22.33.44",
                        serverPath: "/my_dir",
                        credentials: new Credential("username", "password")
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
            }
        }
    }
}
