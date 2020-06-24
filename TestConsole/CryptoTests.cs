using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Application;
using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Crypto;
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
            "Hash File (MD5)",
            "Encrypt Password",
            "Decrypt Password",
            "Encrypt 3 Passwords (random key, IV)",
            "Encrypt 3 Passwords (same key random IV)",
            "Encrypt 3 Passwords (same key, IV)",
        };

        public override void Do()
        {
            Console.WriteLine();
            var selection = PromptMenu
            (
                Menu,
                title: "Crypto Tests"
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
                    break;
                case "Hash File (MD5)":
                    var fileToHashPath = PromptInput("Input file (or drag and drop): ").Replace("\"", "");
                    var fileHash = Hash.File(fileToHashPath, new HashOptions { Algorithm = new System.Security.Cryptography.MD5CryptoServiceProvider() });
                    Console.WriteLine("Hash: " + fileHash);
                    break;
                case "Encrypt Password":
                    var passwordToEncrypt = PromptInput("Enter password to encrypt: ");
                    var encryptedPassword = Encrypt.String(passwordToEncrypt);
                    Console.WriteLine(encryptedPassword);
                    break;
                case "Decrypt Password":
                    var passwordToDecrypt = PromptInput("Enter password to decrypt: ");
                    var decryptedPassword = Decrypt.String(passwordToDecrypt);
                    Console.WriteLine(decryptedPassword);
                    break;
                case "Encrypt 3 Passwords (random key, IV)":
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged();
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                    break;
                case "Encrypt 3 Passwords (same key random IV)":
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = CollectionUtil.Fill(32, fillWith: (byte)32) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                    break;
                case "Encrypt 3 Passwords (same key, IV)":
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = CollectionUtil.Fill(32, fillWith: (byte)32), IV = CollectionUtil.Fill(16, fillWith: (byte)16) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                    break;
            }
        }
    }
}
