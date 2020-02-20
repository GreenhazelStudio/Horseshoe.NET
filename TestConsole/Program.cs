using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Objects;

namespace TestConsole
{
    class Program : ConsoleApp<Program>
    {
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
                FindRoutines(),
                title: "Main Menu",
                autoRun: true
            );
        }

        static void Main(string[] args)
        {
            StartApp();
        }
    }
}
