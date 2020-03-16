﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;
using Horseshoe.NET.OracleDb;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class OracleTests : Routine
    {
        public override Title Title => "Oracle Tests";
        public override bool Looping => true;

        static string Statement { get; set; }

        static OracleTests()
        {
            OracleUtil.UsingStatement += (stmt) => Statement = stmt;
            OracleSettings.SetDefaultConnectionString("user/password@//server:port/service");
        }

        string[] Menu => new string[]
        {
            "Oracle Query"
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
                case "Oracle Query":
                    var oraColumns = new[]
                    {
                        "COL1",
                        "COL2"
                    };
                    using (var conn = OracleUtil.LaunchConnection())
                    {
                        var result = Query.TableOrView.AsObjects
                        (
                            conn,
                            "TABLE1",
                            columns: oraColumns

                        );
                        Console.WriteLine(TextUtil.Dump(result, columnNames: oraColumns));
                        result = Query.TableOrView.AsObjects
                        (
                            conn,
                            "TABLE1",
                            columns: oraColumns,
                            where: Filter.Equals("ACTIVE", 1)
                        );
                        Console.WriteLine(TextUtil.Dump(result, columnNames: oraColumns));
                    }
                    break;
            }
        }
    }
}
