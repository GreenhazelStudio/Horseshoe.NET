using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.ConsoleX;
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
                    "Build Urls"
                },
                title: "SSRS Test Menu"
            );
            switch (selection.SelectedItem)
            {
                case "Build Urls":
                    Console.WriteLine("File: " + ReportUtil.BuildFileUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    Console.WriteLine("Link: " + ReportUtil.BuildHyperlinkUrl("/Accounting/Annual Report", reportServer: "http://localhost", parameters: new Dictionary<string, object> { { "parm1", "ezstr" }, { "parm2", "cr&zy str" } }));
                    break;
            }
        }
    }
}
