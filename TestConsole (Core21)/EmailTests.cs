using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.IO.Email;

namespace TestConsole
{
    class EmailTests : Routine
    {
        public override Title Title => "Email Tests";

        public override bool Looping => true;

        public override IEnumerable<Routine> Menu => new[]
        {
            Routine.Build
            (
                "Plain Email",
                () =>
                {
                    PlainEmail.Send("Horseshoe.NET email test", "This is a plain email test.", to: "recipient@email.com", from: "sender@email.net", connectionInfo: new SmtpConnectionInfo { Server = "smtp-relay@email.biz" });
                    Console.WriteLine("Email sent!");
                }
            )
        };
    }
}
