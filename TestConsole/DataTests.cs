using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class DataTests : Routine
    {
        public override Title Title => "Data Tests";
        public override bool Looping => true;

        string[] Menu => new string[]
        {
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu
            );
            RenderListTitle(selection.SelectedItem);
        }
    }
}
