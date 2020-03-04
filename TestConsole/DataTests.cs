using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;
using Horseshoe.NET.SqlDb;

namespace TestConsole
{
    class DataTests : Routine
    {
        public override Title Title => "Data Tests";
        public override bool Looping => true;

        static string Statement { get; set; }

        static DataTests()
        {
            DataUtil.UsingSqlStatement += (stmt) => Statement = stmt;
        }

        string[] Menu => new string[]
        {
            "Not Equals Test"
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu,
                title: "Menu"
            );
            RenderListTitle(selection.SelectedItem);
            switch(selection.SelectedItem)
            {
                case "Not Equals Test":
                    Update.Table
                    (
                        null,
                        "Table",
                        new[]
                        {
                            new Column("Column", "column")
                        },
                        where: Filter.NotEquals("OtherColumn", 15, columnIsNullable: true)
                    );
                    Console.WriteLine(Statement);
                    break;
            }
        }
    }
}
