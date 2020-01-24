using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.DataAccess;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class DataTests : Routine
    {
        public override Title Title => "Data Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "Encrypt Password",
            "Decrypt Password",
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
                case "Encrypt Password":
                    var passwordToEncrypt = PromptInput("Enter password to encrypt: ");
                    var encryptedPassword = DataUtil.Encrypt(passwordToEncrypt);
                    Console.WriteLine(encryptedPassword);
                    Console.WriteLine();
                    break;
                case "Decrypt Password":
                    var passwordToDecrypt = PromptInput("Enter password to decrypt: ");
                    var decryptedPassword = DataUtil.Decrypt(passwordToDecrypt);
                    Console.WriteLine(decryptedPassword);
                    Console.WriteLine();
                    break;
            }
        }
    }
}
