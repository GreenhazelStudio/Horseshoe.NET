using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class Program : ConsoleApp<Program>
    {
        Routine[] Menu => new Routine[]
        {
            new AppTests()
        };

        public override void Run()
        {
            RenderSplash
            (
                new[]
                {
                    "Welcome to Test Console!",
                    Lib.DisplayName,
                    AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName
                }
            );
            PromptRoutineMenu
            (
                Menu,
                autoRun: true
            );
        }

        static void Main(string[] args)
        {
            StartApp();
        }
    }
}
