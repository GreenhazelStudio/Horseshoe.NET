using System;
using System.Reflection;
using System.Runtime.Versioning;
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
                    Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName
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
