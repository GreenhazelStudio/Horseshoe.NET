using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Objects;

namespace TestConsole
{
    class Program : ConsoleApp
    {
        public override IEnumerable<Routine> MainMenu { get; } = FindRoutines();

        public override IEnumerable<string> SplashMessageLines => new[]
        {
            "Welcome to Test Console!",
            Lib.DisplayName,
            Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
        };

        public override LoopingPolicy LoopingPolicy => LoopingPolicy.ClearScreen | LoopingPolicy.RenderSplash;

        static void Main(string[] args)
        {
            StartConsoleApp<Program>();
        }
    }
}
