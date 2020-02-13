using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.ReportingServices;

namespace TestConsole
{
    class SsrsTests : Routine
    {
        public override Title Title => "SSRS Tests";
        public override bool Looping => true;
        public override void Do()
        {
            var selection = PromptMenu
            (
                new[]
                {
                    "Build Urls",
                    "Launch Test Report",
                    "Launch Report",
                    "Launch Report w/ '&'",
                },
                title: "SSRS Test Menu"
            );
            switch (selection.SelectedItem)
            {
                case "Build Urls":
                    Console.WriteLine("File: " + ReportUtil.BuildFileUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    Console.WriteLine("Link: " + ReportUtil.BuildHyperlinkUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    break;
                case "Launch Test Report":
                    Console.WriteLine("Downloading...");
                    var testReport = ReportServer.Render("/Operations/MustInherit/Sandbox/ParamTest", reportServer: "https://reports.derbyllc.com", parameters: new Dictionary<string, object> { { "param1", "12 34 & 56" } }, targetFileName: "param-test.pdf", targetDirectory: Path.GetTempPath());
                    Console.WriteLine(FileUtil.GetDisplayFileSize(new FileInfo(testReport)));
                    Console.WriteLine("Launch...");
                    Process.Start(testReport);
                    break;
                case "Launch Report":
                    Console.WriteLine("Downloading...");
                    var report = ReportServer.Render("/Operations/Division 13/Navision Reports/Vendor Planning Schedule Flex (Bulk Email)", reportServer: "https://reports.derbyllc.com", parameters: new Dictionary<string, object> { { "Vendor", "ECIM-13" }, { "Worksheet", "GBGWPL" }, { "CutoffDays", "352" } }, targetFileName: "report.pdf", targetDirectory: Path.GetTempPath());
                    Console.WriteLine(FileUtil.GetDisplayFileSize(new FileInfo(report)));
                    Console.WriteLine("Launch...");
                    Process.Start(report);
                    break;
                case "Launch Report w/ '&'":
                    Console.WriteLine("Downloading...");
                    var reportwamp = ReportServer.Render("/Operations/Division 13/Navision Reports/Vendor Planning Schedule Flex (Bulk Email)", reportServer: "https://reports.derbyllc.com", parameters: new Dictionary<string, object> { { "Vendor", "E&OTO-13" }, { "Worksheet", "GBGWPL" }, { "CutoffDays", "352" } }, targetFileName: "report.pdf", targetDirectory: Path.GetTempPath());
                    Console.WriteLine(FileUtil.GetDisplayFileSize(new FileInfo(reportwamp)));
                    Console.WriteLine("Launch...");
                    Process.Start(reportwamp);
                    break;
            }
        }
    }
}
