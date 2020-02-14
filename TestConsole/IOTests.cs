using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO;
using Horseshoe.NET.IO.ReportingServices;

namespace TestConsole
{
    class IOTests : Routine
    {
        public override Title Title => "IO Tests";
        public override bool Looping => true;
        public override void Do()
        {
            var selection = PromptMenu
            (
                new[]
                {
                    "Build SSRS URLs",
                    "Display file sizes"
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
            }
        }
    }
}
