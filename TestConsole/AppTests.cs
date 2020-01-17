using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
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
            "Console Properties",
            "Environment Properties",
            "AppDomain Properties",
            "Assemblies",
            "Detect App Type",
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu
            );
            RenderListTitle(selection.SelectedItem);
            switch (selection.SelectedItem)
            {
                case "Console Properties":
                    var consoleProperties = typeof(Console).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    foreach (var prop in consoleProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            Console.WriteLine(TextUtil.RevealNullOrBlank(prop.GetValue(null)));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                    break;
                case "Environment Properties":
                    var environmentProperties = typeof(Environment).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    foreach (var prop in environmentProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            if (prop.Name.Equals("NewLine"))
                            {
                                Console.WriteLine(TextUtil.RevealWhiteSpace(prop.GetValue(null)));
                            }
                            else if (prop.Name.Equals("StackTrace"))
                            {
                                Console.WriteLine(((string)prop.GetValue(null)).Substring(0, 70) + "...");
                            }
                            else
                            {
                                Console.WriteLine(TextUtil.RevealNullOrBlank(prop.GetValue(null)));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                    break;
                case "AppDomain Properties":
                    var appDomainProperties = typeof(AppDomain).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    foreach (var prop in appDomainProperties)
                    {
                        Console.Write(prop.Name.PadRight(25) + " ");
                        try
                        {
                            if (prop.Name.Equals("PermissionSet"))
                            {
                                Console.WriteLine(TextUtil.RevealNullOrBlank(prop.GetValue(AppDomain.CurrentDomain)?.ToString().Replace(Environment.NewLine, " ").Trim()));
                            }
                            else
                            {
                                Console.WriteLine(TextUtil.RevealNullOrBlank(prop.GetValue(AppDomain.CurrentDomain)));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().Name);
                        }
                    }
                    break;
                case "Assemblies":
                    RenderList(AppDomain.CurrentDomain.GetAssemblies(), renderer: (a) => a.FullName);
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
