using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections;
using Horseshoe.NET.ConsoleX;
using Horseshoe.NET.Crypto;
using Horseshoe.NET.Text;
using Horseshoe.NET.Text.Extensions;

namespace TestConsole
{
    class CryptoTests : Routine
    {
        public override Title Title => "Crypto Tests";

        public override RoutineTitleRenderPolicy RenderTitlePolicy => RoutineTitleRenderPolicy.RenderOnLoop;

        public override Action<MenuSelection<Routine>> OnMenuSelectionRunComplete => (selection) => PromptContinue(padBefore: 2);

        public override bool Looping => true;

        public override bool ClearScreenOnLoop => true;

        public override IEnumerable<Routine> Menu => new[]
        {
            Routine.Build
            (
                "Hash (Default)",
                () =>
                {
                    var input = PromptInput("Enter text to hash: ");
                    var hash = Hash.String(input);
                    Console.WriteLine(hash);
                }
            ),
            Routine.Build
            (
                "Hash (SHA256 + salt)",
                () =>
                {
                    var input2 = PromptInput("Enter text to hash: ");
                    var hash2 = Hash.String(input2, new HashOptions { Algorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider(), Salt = 112 });
                    Console.WriteLine(hash2);
                }
            ),
            Routine.Build
            (
                "Hash (Default => SHA256 + salt)",
                () =>
                {
                    CryptoSettings.DefaultHashAlgorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                    CryptoSettings.DefaultHashSalt = 112;
                    var input3 = PromptInput("Enter text to hash: ");
                    var hash3 = Hash.String(input3);
                    Console.WriteLine(hash3);
                    CryptoSettings.DefaultHashAlgorithm = null;
                    CryptoSettings.DefaultHashSalt = null;
                }
            ),
            Routine.Build
            (
                "Hash File (MD5)",
                () =>
                {
                    var fileToHashPath = PromptInput("Input file (or drag and drop): ").Replace("\"", "");
                    var fileHash = Hash.File(fileToHashPath, new HashOptions { Algorithm = new System.Security.Cryptography.MD5CryptoServiceProvider() });
                    Console.WriteLine("Hash: " + fileHash);
                }
            ),
            Routine.Build
            (
                "Encrypt Password",
                () =>
                {
                    var passwordToEncrypt = PromptInput("Enter password to encrypt: ");
                    var encryptedPassword = Encrypt.String(passwordToEncrypt);
                    Console.WriteLine(encryptedPassword);
                }
            ),
            Routine.Build
            (
                "Decrypt Password",
                () =>
                {
                    var passwordToDecrypt = PromptInput("Enter password to decrypt: ");
                    var decryptedPassword = Decrypt.String(passwordToDecrypt);
                    Console.WriteLine(decryptedPassword);
                }
            ),
            Routine.Build
            (
                "Encrypt 3 Passwords (random key, IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged();
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            ),
            Routine.Build
            (
                "Encrypt 3 Passwords (same key random IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = CollectionUtil.Fill(32, fillWith: (byte)32) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            ),
            Routine.Build
            (
                "Encrypt 3 Passwords (same key, IV)",
                () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var algorithm = new System.Security.Cryptography.RijndaelManaged { Key = CollectionUtil.Fill(32, fillWith: (byte)32), IV = CollectionUtil.Fill(16, fillWith: (byte)16) };
                        var ciphertext = Encrypt.String("This is your life!", new CryptoOptions { Algorithm = algorithm });
                        Console.WriteLine("This is your life!" + " -> " + ciphertext.Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- Key: " + string.Join(", ", algorithm.Key).Crop(26, truncateMarker: TruncateMarker.LongEllipsis) + " -- IV: " + string.Join(", ", algorithm.IV).Crop(26, truncateMarker: TruncateMarker.LongEllipsis));
                    }
                }
            )
        };
    }
}
