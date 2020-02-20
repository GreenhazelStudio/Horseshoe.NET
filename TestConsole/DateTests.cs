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
    class DateTests : Routine
    {
        public override Title Title => "Date Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "Max Age",
            "Compare DateDiff and TimeSpan",
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
                case "Compare DateDiff and TimeSpan":
                    var compareDates = new[] { new DateTime(2019, 5, 12), new DateTime(2010, 7, 4), new DateTime(1979, 6, 27) };
                    foreach(var date in compareDates)
                    {
                        var ddYears = date.GetAgeInYears();
                        var ddPYears = date.GetPreciseAgeInYears();
                        var span = DateTime.Now - date;

                    }
                    break;
            }
        }
    }
}
