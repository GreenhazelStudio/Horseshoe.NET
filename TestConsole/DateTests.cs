using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Extensions;
using Horseshoe.NET.ConsoleX;

namespace TestConsole
{
    class DateTests : Routine
    {
        public override Title Title => "Date Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "Max Age",
            "Compare ages and timespans",
            "Display different ages",
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu,
                title: "Date Tests"
            );
            RenderListTitle(selection.SelectedItem);
            var now = DateTime.Now;
            switch (selection.SelectedItem)
            {
                case "Max Age":
                    Console.WriteLine("int " + int.MaxValue);
                    var maxIntAgeInDays = (double)int.MaxValue / (1000 * 60 * 60 * 24);
                    var maxIntAgeInYears = maxIntAgeInDays / 365;
                    Console.WriteLine(string.Format("max age: {0:0.##} days or {1:0.##} years", maxIntAgeInDays, maxIntAgeInYears));
                    Console.WriteLine();
                    Console.WriteLine("long " + long.MaxValue);
                    var maxLongAgeInDays = (double)long.MaxValue / (1000 * 60 * 60 * 24);
                    var maxLongAgeInYears = maxLongAgeInDays / 365;
                    Console.WriteLine(string.Format("max age: {0:0.##} days or {1:0.##} years", maxLongAgeInDays, maxLongAgeInYears));
                    break;
                case "Compare ages and timespans":
                    var compareDates = new[] { new DateTime(2019, 5, 12), new DateTime(2010, 7, 4), new DateTime(1979, 6, 27) };
                    foreach (var date in compareDates)
                    {
                        var age = date.GetAge(asOf: now, decimals: 3);
                        var span = now - date;

                        RenderListTitle(date.ToString(), padBefore: 1);
                        Console.WriteLine("  * Age: " + age);
                        Console.WriteLine("    - Years      : " + age.Years);
                        Console.WriteLine("    - TotalYears : " + age.TotalYears);
                        Console.WriteLine("    - Months     : " + age.Months);
                        Console.WriteLine("    - TotalMonths: " + age.TotalMonths);
                        Console.WriteLine("    - Days       : " + age.Days);
                        Console.WriteLine("    - TotalDays  : " + age.TotalDays);
                        Console.WriteLine("    - Hours      : " + age.Hours);
                        Console.WriteLine("    - TotalHours : " + age.TotalHours);
                        Console.WriteLine("  * Span: " + span);
                        Console.WriteLine("    - Days       : " + span.Days);
                        Console.WriteLine("    - TotalDays  : " + span.TotalDays);
                        Console.WriteLine("    - Hours      : " + span.Hours);
                        Console.WriteLine("    - TotalHours : " + span.TotalHours);
                    }
                    break;
                case "Display different ages":
                    var compareAgeDates = new[] { new DateTime(2019, 5, 12), new DateTime(2010, 7, 4), new DateTime(1979, 6, 27) };
                    foreach (var date in compareAgeDates)
                    {
                        RenderListTitle(date.ToString(), padBefore: 1);
                        Console.WriteLine("  * Age           : " + date.GetAge(asOf: now, decimals: 3));
                        Console.WriteLine("  * Years         : " + date.GetAgeInYears(asOf: now));
                        Console.WriteLine("  * Total Years   : " + date.GetTotalAgeInYears(asOf: now, decimals: 3));
                        Console.WriteLine("  * Months        : " + date.GetAgeInMonths(asOf: now));
                        Console.WriteLine("  * Total Months  : " + date.GetTotalAgeInMonths(asOf: now, decimals: 3));
                        Console.WriteLine("  * Days          : " + date.GetAgeInDays(asOf: now));
                        Console.WriteLine("  * Total Days    : " + date.GetTotalAgeInDays(asOf: now, decimals: 3));
                    }
                    break;
            }
        }
    }
}
