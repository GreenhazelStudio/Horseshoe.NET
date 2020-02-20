using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

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
                    Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
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
