using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class AppTests : Routine
    {
        public override Title Title => "App Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "List App Type Indicators",
            "Detect App Type",
        };

        public override void Do()
        {
            Console.WriteLine("Environment.OSVersion: " + Environment.OSVersion);
            Console.WriteLine("Environment.Version: " + Environment.Version);
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu
            );
            Console.WriteLine(selection.SelectedItem);
            Console.WriteLine();
            switch (selection.SelectedItem)
            {
                case "List App Type Indicators":
                    //var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    //Console.WriteLine("Assemblies");
                    //RenderList(appAssemblies, title: "Assemblies", renderer: (a) => a.FullName);
                    var consoleProps = typeof(Console).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Static);
                    RenderList(consoleProps, title: "Console Properties", renderer: (p) => p.Name.PadRight(28) + " " + TextUtil.Reveal(p.GetValue(null), nullOrBlank: true, crlf: true));
                    var environmentProps = typeof(Environment).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Static);
                    RenderList(environmentProps, title: "Environment Properties", renderer: (p) => p.Name.PadRight(28) + " " + TextUtil.Reveal(p.GetValue(null), nullOrBlank: true, crlf: true));
                    var appDomainProps = typeof(AppDomain).GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.Instance);
                    RenderList(appDomainProps, title: "App Domain Properties", renderer: (p) => p.Name.PadRight(28) + " " + TextUtil.Reveal(p.GetValue(AppDomain.CurrentDomain), nullOrBlank: true, crlf: true));
                    RenderListTitle("Process Properties");
                    Console.WriteLine("MainWindowHandle".PadRight(28) + " " + Process.GetCurrentProcess().MainWindowHandle);
                    Console.WriteLine("MainWindowTitle".PadRight(28) + " " + Process.GetCurrentProcess().MainWindowTitle);
                    break;
                case "Detect App Type":
                    var sb = new StringBuilder();
                    var appType = ClientApp.DetectAppType(sb);
                    RenderListTitle("Detected: " + (appType?.ToString() ?? "[null]"));
                    Console.WriteLine(sb);
                    break;
            }
        }
    }
}
