using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Db;
using Horseshoe.NET.SqlDb;

namespace TestConsole
{
    class SqlTests : Routine
    {
        public override Title Title => "Sql Tests";

        public override bool Looping => true;

        public override IEnumerable<Routine> Menu => new []
        {
            Routine.Build
            (
                "Not Equals Test",
                () =>
                {
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
                }
            )
        };

        static string Statement { get; set; }

        static SqlTests()
        {
            SqlUtil.UsingStatement += (stmt) => Statement = stmt;
        }
    }
}
