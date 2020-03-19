using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ActiveDirectory;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class ADTests : Routine
    {
        public override Title Title => "AD Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "What is my domain controller?"
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu
            );
            RenderListTitle(selection.SelectedItem);
            switch (selection.SelectedItem)
            {
                case "What is my domain controller?":
                    Console.WriteLine("My domain controller: " + TextUtil.RevealNullOrBlank(ADUtil.DetectDomainController()));
                    break;
            }
        }
    }
}
