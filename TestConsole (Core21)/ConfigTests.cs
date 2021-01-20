using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class ConfigTests : Routine
    {
        public override Title Title => "Config Tests";

        public override RoutineTitleRenderPolicy RenderTitlePolicy => RoutineTitleRenderPolicy.RenderOnLoop;

        public override Action<MenuSelection<Routine>> OnMenuSelectionRunComplete => (selection) => PromptContinue(padBefore: 2);

        public override bool Looping => true;

        public override bool ClearScreenOnLoop => false;

        public override IEnumerable<Routine> Menu => new[]
        {
            Routine.Build
            (
                "List config collections",
                () =>
                {
                    var array = Config.GetArray<Element>("MyConfigTestSection:Elements", required: true);
                    RenderMenuTitle("Collection");
                    foreach(var element in array)
                    {
                        Console.WriteLine("Foo = " + element.Foo + "; Bar = " + element.Bar);
                    }
                    Console.WriteLine();
                }
            ),
        };

        public class Element
        {
            public string Foo { get; set; }

            public string Bar { get; set; }
        }
    }
}