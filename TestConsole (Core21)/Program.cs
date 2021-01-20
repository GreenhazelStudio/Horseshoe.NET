using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

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

        public override SplashRenderPolicy SplashRenderPolicy => SplashRenderPolicy.RenderOnLoop;

        public override bool ClearScreenOnLoop => true;

        static void Main(string[] args)
        {
            var root = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Horseshoe.NET.Application.Config.LoadConfigurationService(root);
            StartConsoleApp<Program>();
        }
    }
}
