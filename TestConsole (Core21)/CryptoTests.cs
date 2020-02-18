using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Cryptography;
using Horseshoe.NET.Text;

namespace TestConsole
{
    class CryptoTests : Routine
    {
        public override Title Title => "Crypto Tests";
        public override bool Looping => true;

        string[] Menu => new[]
        {
            "Hash (Default)",
            "Hash (SHA256 + salt)",
            "Hash (Default => SHA256 + salt)",
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
            CryptoSettings.DefaultHashAlgorithm = null;
            CryptoSettings.DefaultHashSalt = null;
            switch (selection.SelectedItem)
            {
                case "Hash (Default)":
                    var input = PromptInput("Enter text to hash: ");
                    var hash = Hash.String(input);
                    Console.WriteLine(hash);
                    Console.WriteLine();
                    break;
                case "Hash (SHA256 + salt)":
                    var input2 = PromptInput("Enter text to hash: ");
                    var hash2 = Hash.String(input2, new HashOptions { Algorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider(), Salt = 112 });
                    Console.WriteLine(hash2);
                    Console.WriteLine();
                    break;
                case "Hash (Default => SHA256 + salt)":
                    CryptoSettings.DefaultHashAlgorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                    CryptoSettings.DefaultHashSalt = 112;
                    var input3 = PromptInput("Enter text to hash: ");
                    var hash3 = Hash.String(input3);
                    Console.WriteLine(hash3);
                    Console.WriteLine();
                    break;
                case "Encrypt Password":
                    var passwordToEncrypt = PromptInput("Enter password to encrypt: ");
                    var encryptedPassword = Encrypt.String(passwordToEncrypt);
                    Console.WriteLine(encryptedPassword);
                    Console.WriteLine();
                    break;
                case "Decrypt Password":
                    var passwordToDecrypt = PromptInput("Enter password to decrypt: ");
                    var decryptedPassword = Decrypt.String(passwordToDecrypt);
                    Console.WriteLine(decryptedPassword);
                    Console.WriteLine();
                    break;
            }
        }
    }
}
