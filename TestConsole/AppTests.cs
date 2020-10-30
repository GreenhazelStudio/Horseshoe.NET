using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class AppTests : Routine
    {
        public override Title Title => "App Tests";

        public override bool Looping => true;

        public override IEnumerable<Routine> Menu => new[]
        {
            Routine.Build
            (
                "Console Properties",
                () =>
                {
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
                }
            ),
            Routine.Build
            (
                "Environment Properties",
                () =>
                {
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
                }
            ),
            Routine.Build
            (
                "AppDomain Properties",
                () =>
                {
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
                }
            ),
            Routine.Build
            (
                "Assemblies",
                () =>
                {
                    RenderList(AppDomain.CurrentDomain.GetAssemblies(), renderer: (a) => a.FullName);
                }
            ),
            Routine.Build
            (
                "Detect App Type",
                () =>
                {
                    var sb = new StringBuilder();
                    var appType = ClientApp.DetectAppType(sb);
                    RenderListTitle("Detected: " + (appType?.ToString() ?? "[null]"));
                    Console.WriteLine(sb);
                }
            )
        };
    }
}
