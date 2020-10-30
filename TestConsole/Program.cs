using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class Program : ConsoleApp
    {
        public override IEnumerable<Routine> MainMenu { get; } = FindRoutines();

        public override IEnumerable<string> SplashMessageLines => new[]
        {
            "Welcome to Test Console!",
            Lib.DisplayName,
            AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName
        };

        public override LoopingPolicy LoopingPolicy => LoopingPolicy.ClearScreen | LoopingPolicy.RenderSplash;

        static void Main(string[] args)
        {
            StartConsoleApp<Program>();
        }
    }
}
