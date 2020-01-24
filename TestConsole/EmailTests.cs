using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Email;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class EmailTests : Routine
    {
        public override Title Title => "Email Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "Plain Email",
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
                case "Plain Email":
                    PlainEmail.Send("This is a plain email test.", "Horseshoe.NET email test", to: "carl.thomas@derbyllc.com", from: "no-reply@deryllc.com");
                    Console.WriteLine("Email sent!");
                    break;
            }
        }
    }
}
