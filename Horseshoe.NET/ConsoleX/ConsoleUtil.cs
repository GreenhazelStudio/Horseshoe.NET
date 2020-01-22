using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.ConsoleX
{
    public static class ConsoleUtil
    {
        private static int ConsoleWidth { get => Console.WindowWidth - 2; }

        public static void RenderSplash(string welcomeMessage, int padBefore = 1, int padAfter = 2)
        {
            var welcomeMessageLines = welcomeMessage.Replace(Environment.NewLine, "\n")
                .Split('\n')
                .ZapAndPrune()
                .ToArray();
            RenderSplash(welcomeMessageLines, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderSplash(string[] welcomeMessageLines, int padBefore = 1, int padAfter = 2)
        {
            Pad(padBefore);
            Console.WriteLine(" »╔" + "".PadLeft(ConsoleWidth - 4, '═') + "╗«");
            Console.WriteLine(" »║" + "".PadLeft(ConsoleWidth - 4, ' ') + "║«");
            foreach(var line in welcomeMessageLines)
            {
                Console.WriteLine(" »║" + line.PadCenter(ConsoleWidth - 4) + "║«");
            }
            Console.WriteLine(" »║" + "".PadLeft(ConsoleWidth - 4, ' ') + "║«");
            Console.WriteLine(" »╚" + "".PadLeft(ConsoleWidth - 4, '═') + "╝«");
            Pad(padAfter);
        }

        public static void RenderRoutineTitle(Title title, int padBefore = 1, int padAfter = 1)
        {
            Pad(padBefore);
            Console.WriteLine(" ╔" + "".PadLeft(ConsoleWidth - 2, '═') + "╗");
            Console.WriteLine(" ║" + title.ToString().PadCenter(ConsoleWidth - 2) + '║');
            Console.WriteLine(" ╚" + "".PadLeft(ConsoleWidth - 2, '═') + "╝");
            Pad(padAfter);
        }

        public static void RenderMenuTitle(Title title, int padBefore = 0, int padAfter = 0)
        {
            Pad(padBefore);
            Console.WriteLine(title.Render());
            Console.WriteLine("".PadLeft(title.Text.Length, '='));
            Pad(padAfter);
        }

        public static void RenderListTitle(Title title, int padBefore = 0, int padAfter = 0)
        {
            Pad(padBefore);
            Console.WriteLine(title.Render());
            Console.WriteLine("".PadLeft(title.Text.Length, '-'));
            Pad(padAfter);
        }

        public static void RenderMenu<E>
        (
            IEnumerable<E> menuItems,
            Func<E, string> renderer = null,
            int padBefore = 0,
            Title? title = null,
            IEnumerable<CustomMenuItem> customMenuItems = null,
            int padAfter = 1
        )
        {
            // render title
            if (title.HasValue)
            {
                RenderMenuTitle(title.Value, padBefore: padBefore);
            }
            else
            {
                Pad(padBefore);
            }

            if ((menuItems == null || !menuItems.Any()) && (customMenuItems == null || !customMenuItems.Any()))
            {
                Console.WriteLine("[There are no menu items to display]");
                return;
            }

            // declare uniformer
            var menuItemUniformer = new LRUniformer();

            // add menu items
            if (menuItems != null)
            {
                var counter = 0;
                foreach (var item in menuItems)
                {
                    menuItemUniformer.AddUniqueMenuItem(++counter, renderer != null ? renderer.Invoke(item) : item?.ToString(), lIsIndex: true);
                }
            }

            // add custom menu items
            if (customMenuItems != null)
            {
                var prependedCount = 0;
                foreach (var custItem in customMenuItems)
                {
                    if (custItem.PrependToMenu)
                    {
                        menuItemUniformer.InsertUniqueMenuItem(prependedCount++, TextUtil.Zap(custItem.Command) ?? "<┘", custItem.Text);
                    }
                    else
                    {
                        menuItemUniformer.AddUniqueMenuItem(TextUtil.Zap(custItem.Command) ?? "<┘", custItem.Text);
                    }
                }
            }

            Console.Write(menuItemUniformer.RenderLSquareBrackets());
            Pad(padAfter);
        }

        /// <summary>
        /// Displays a menu in the console and waits for user input.
        /// </summary>
        public static MenuSelection<object> PromptMenu
        (
            Title? title = null,
            IEnumerable<CustomMenuItem> customMenuItems = null,
            string prompt = ">",
            int padBefore = 0,
            int padAfter = 1,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            bool allowExitApplication = true
        )
        {
            return PromptMenu
            (
                null as IEnumerable<object>,
                title: title,
                customMenuItems: customMenuItems,
                prompt: prompt,
                padBefore: padBefore,
                padAfter: padAfter,
                allowArbitraryInput: allowArbitraryInput,
                allowMultipleSelection: allowMultipleSelection,
                allowExitApplication: allowExitApplication
            );
        }

        /// <summary>
        /// Displays a menu in the console and waits for user input.
        /// </summary>
        public static MenuSelection<E> PromptMenu<E>
        (
            IEnumerable<E> menuItems,
            Func<E, string> renderer = null,
            Title? title = null,
            IEnumerable<CustomMenuItem> customMenuItems = null,
            string prompt = ">",
            int padBefore = 0,
            int padAfter = 1,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            bool allowExitApplication = true
        ) where E : class
        {
            ValidateMenu
            (
                menuItems,
                customMenuItems: customMenuItems
            );

            RenderMenu
            (
                menuItems,
                renderer: renderer,
                padBefore: padBefore,
                title: title,
                customMenuItems: customMenuItems,
                padAfter: padAfter
            );

            while (true)
            {
                var rawInput = PromptInput(prompt: prompt, allowExitApplication: allowExitApplication);
                var input = rawInput.Trim();
                if (customMenuItems != null)
                {
                    foreach (var custItem in customMenuItems)
                    {
                        if (string.Equals(input, custItem.Command, StringComparison.CurrentCultureIgnoreCase) && !custItem.Inert)
                        {
                            if (custItem.Action != null)
                            {
                                custItem.Action.Invoke();
                                return new MenuSelection<E>();
                            }
                            return new MenuSelection<E> { CustomMenuItem = custItem };
                        }
                    }
                }
                if (allowMultipleSelection && input.Length != 0)
                {
                    try
                    {
                        var selectedIndexes = MenuSelection.ParseMultipleIndexes(input, menuItems.Count(), out bool all);
                        var selectedItems = selectedIndexes
                            .Select(i => menuItems.ElementAt(i - 1))
                            .ToArray();
                        return new MenuSelection<E>
                        {
                            SelectedIndexes = selectedIndexes,
                            SelectedItems = selectedItems,
                            SelectedAll = all
                        };
                    }
                    catch (BenignException ex)
                    {
                        RenderAlert(ex.Message);
                    }
                }
                else if (int.TryParse(input, out int index))
                {
                    try
                    {
                        return new MenuSelection<E>
                        {
                            SelectedIndex = index,
                            SelectedItem = menuItems.ElementAt(index - 1)   // convert back to 0-based index;  uses IndexOutOfRangeException for autovalidation
                        };
                    }
                    catch (Exception)
                    {
                        if (allowArbitraryInput)
                        {
                            return new MenuSelection<E> { ArbitraryInput = rawInput };
                        }
                        else
                        {
                            RenderAlert("Invalid menu selection");
                        }
                    }
                }
                else if (allowArbitraryInput && input.Length != 0)
                {
                    return new MenuSelection<E> { ArbitraryInput = rawInput };
                }
                else
                {
                    RenderAlert("Invalid menu selection");
                }
            }
        }

        private static void ValidateMenu<E>
        (
            IEnumerable<E> menuItems,
            IEnumerable<CustomMenuItem> customMenuItems
        )
        {
            var menuItemsForCompare = new Dictionary<string, string>();
            if (customMenuItems != null && customMenuItems.Any())
            {
                foreach (var custItem in customMenuItems)
                {
                    if (menuItemsForCompare.ContainsKeyIgnoreCase(custItem.Command, out KeyValuePair<string, string> matchingKvp))
                    {
                        throw new ValidationException("Menu commands must be unique: " + custItem.Command + " - " + custItem.Text + " ... " + matchingKvp.Key + " - " + matchingKvp.Value);
                    }
                    if (int.TryParse(custItem.Command, out int checkIndex) && menuItems != null)
                    {
                        if (checkIndex >= 1 && checkIndex <= menuItems.Count())
                        {
                            throw new ValidationException("Menu commands must be unique: " + custItem.Command + " - " + custItem.Text + " ... " + checkIndex + " - " + menuItems.ElementAt(checkIndex - 1));
                        }
                    }
                    menuItemsForCompare.Add(custItem.Command, custItem.Text);
                }
            }
        }

        /// <summary>
        /// Prompts a user for input and returns the line of text typed by the user.
        /// </summary>
        /// <returns>The line of text typed by the user</returns>
        public static string PromptInput(int padBefore = 0, int padAfter = 1, bool allowExitApplication = false)
        {
            return PromptInput(">", padBefore: padBefore, padAfter: padAfter, allowExitApplication: allowExitApplication);
        }

        /// <summary>
        /// Prompts a user for input and returns the line of text typed by the user.
        /// </summary>
        /// <returns>The line of text typed by the user</returns>
        public static string PromptInput(string prompt, int padBefore = 0, int padAfter = 1, bool allowExitApplication = false)
        {
            Pad(padBefore);
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (input.Equals("EXIT", StringComparison.OrdinalIgnoreCase) && allowExitApplication)
            {
                Navigation.ExitApp();
            }
            Pad(padAfter);
            return input;
        }

        /// <summary>
        /// Pause and wait for acknowledgement
        /// </summary>
        /// <param name="prompt"></param>
        public static void PromptContinue(int padBefore = 0, int padAfter = 0)
        {
            PromptContinue("Press any key to continue...", padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Pause and wait for acknowledgement
        /// </summary>
        public static void PromptContinue(string prompt, int padBefore = 0, int padAfter = 0)
        {
            Pad(padBefore);
            Console.WriteLine(prompt);
            Console.ReadKey();
            Pad(padAfter);
        }

        /// <summary>
        /// Pause and wait for acknowledgement before exiting
        /// </summary>
        public static void PromptExit(int padBefore = 0, int padAfter = 0)
        {
            PromptContinue("Press any key to exit...", padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Pause and wait for acknowledgement before exiting
        /// </summary>
        public static void PromptExit(string prompt, int padBefore = 0, int padAfter = 0)
        {
            PromptContinue(prompt, padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Prompts a user for a password, hiding it from the console display
        /// </summary>
        public static string PromptPassword(int padBefore = 0, int padAfter = 1)
        {
            return PromptPassword(">", padBefore: padBefore, padAfter: padAfter);
        }

        /// <summary>
        /// Prompts a user for a password, hiding it from the console display
        /// </summary>
        public static string PromptPassword(string prompt, int padBefore = 0, int padAfter = 1)
        {
            var plainTextBuilder = new StringBuilder();

            Pad(padBefore);
            Console.Write(prompt);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    Pad(padAfter);
                    Navigation.CancelPassword();
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (plainTextBuilder.Length > 0)
                    {
                        plainTextBuilder.Length -= 1;
                        Console.Write("\b \b");
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else if (keyInfo.IsCursorNavigation())
                {
                    Console.Beep();
                }
                else if (keyInfo.IsLetterOrDigit() || keyInfo.IsPunctuation() || keyInfo.IsSpecialCharacter() || keyInfo.IsSpace())
                {
                    plainTextBuilder.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    RenderAlert("Unexpected input: " + keyInfo.Key, padBefore: 2, padAfter: 2);
                    return PromptPassword(prompt, padBefore: padBefore, padAfter: padAfter);
                }
            }
            Pad(padAfter);
            return plainTextBuilder.ToString();
        }

        /// <summary>
        /// Prompts a user for a password, hiding it from the console display
        /// </summary>
        public static SecureString PromptPasswordSecure(int padBefore = 0, int padAfter = 1)
        {
            return PromptPasswordSecure(">", padBefore: padBefore, padAfter: padAfter);
        }


        /// <summary>
        /// Prompts a user for a password, hiding it from the console display
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0068:Use recommended dispose pattern", Justification = "Client code must perform secure string disposal")]
        public static SecureString PromptPasswordSecure(string prompt, int padBefore = 0, int padAfter = 1)
        {
            var secureString = new SecureString();

            Pad(padBefore);
            Console.Write(prompt);

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    Pad(padAfter);
                    Navigation.CancelPassword();
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (secureString.Length > 0)
                    {
                        secureString.RemoveAt(secureString.Length - 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        Console.Beep();
                    }
                }
                else if (keyInfo.IsCursorNavigation())
                {
                    Console.Beep();
                }
                else if (keyInfo.IsLetterOrDigit() || keyInfo.IsPunctuation() || keyInfo.IsSpecialCharacter() || keyInfo.IsSpace())
                {
                    secureString.AppendChar(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    RenderAlert("Unexpected input: " + keyInfo.Key, padBefore: 2, padAfter: 2);
                    return PromptPasswordSecure(prompt, padBefore: padBefore, padAfter: padAfter);
                }
            }
            Pad(padAfter);
            secureString.MakeReadOnly();
            return secureString;
        }

        /// <summary>
        /// Dump an exception to the console.  You can configure this to iterate through and display all inner exceptions as well. 
        /// </summary>
        /// <param name="ex">A .NET exception</param>
        /// <param name="recursive">Whether to iterate through the inner exceptions as well</param>
        /// <param name="includeStackTrace">Whether to output the exception's stack trace</param>
        public static void RenderException(Exception ex, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, bool recursive = false, int padBefore = 0, int padAfter = 1)
        {
            Pad(padBefore);
            Console.WriteLine
            (
                ex.Render(displayShortName: displayShortName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, recursive: recursive)
            );
            Pad(padAfter);
        }

        /// <summary>
        /// Displays a brief message centered on the console
        /// </summary>
        /// <param name="message"></param>
        public static void RenderAlert(string message, int padBefore = 1, int padAfter = 1)
        {
            message = "** " + (message ?? "Alert!") + " **";
            message = message.PadCenter(ConsoleWidth - 2);
            Pad(padBefore);
            Console.WriteLine(message);
            Pad(padAfter);
        }

        /// <summary>
        /// Output a list of items to the console
        /// </summary>
        /// <param name="strings">a list of items</param>
        /// <param name="options">different ways to customize the rendering</param>
        public static void RenderList<E>
        (
            IEnumerable<E> list,
            ListIndexPolicy indexPolicy = ListIndexPolicy.NotDisplayed,
            Func<E, string> renderer = null,
            Title? title = null,
            int padBefore = 0,
            int padAfter = 1
        )
        {
            if (title.HasValue)
            {
                RenderListTitle(title.Value, padBefore: padBefore);
            }
            else
            {
                Pad(padBefore);
            }
            if (list == null)
            {
                Console.WriteLine("[null list]");
            }
            else if (!list.Any())
            {
                Console.WriteLine("[0-length list]");
            }
            else
            {
                var index = 0;

                switch (indexPolicy)
                {
                    case ListIndexPolicy.DisplayZeroBased:
                        var listUniformer0 = new LRUniformer();
                        foreach (var item in list)
                        {
                            listUniformer0.Add(index++, (renderer != null ? renderer.Invoke(item) : item?.ToString()) ?? "[null]");
                        }
                        var pad0 = listUniformer0.MaxLWidth + 2;  // add 2 for []
                        foreach (var item in listUniformer0)
                        {
                            Console.Write(("[" + item.L + "]").PadLeft(pad0));
                            Console.Write(" ");                   // guaranteed space between index and item
                            Console.WriteLine(item.R);
                        }
                        break;
                    case ListIndexPolicy.DisplayOneBased:
                        var listUniformer1 = new LRUniformer();
                        foreach (var item in list)
                        {
                            listUniformer1.Add(++index, (renderer != null ? renderer.Invoke(item) : item?.ToString()) ?? "[null]");
                        }
                        var pad1 = listUniformer1.MaxLWidth + 2;  // add 2 for []
                        foreach (var item in listUniformer1)
                        {
                            Console.Write(("[" + item.L + "]").PadLeft(pad1));
                            Console.Write(" ");                   // guaranteed space between index and item
                            Console.WriteLine(item.R);
                        }
                        break;
                    default:
                        foreach (var item in list)
                        {
                            Console.WriteLine((renderer != null ? renderer.Invoke(item) : item?.ToString()) ?? "[null]");
                            index++;
                        }
                        break;
                }
            }
            Pad(padAfter);
        }

        static void Pad(int pad, string altText = null)
        {
            for (int i = 0; i < pad; i++)
            {
                Console.WriteLine(altText);
            }
        }
    }

    internal static class ConsoleUtilExtensions
    {
        internal static bool ContainsKeyIgnoreCase(this Dictionary<string, string> dictionary, string searchKey, out KeyValuePair<string, string> matchingKvp)
        {
            var _matchingKvp = GetByKeyIgnoreCase(dictionary, searchKey);
            if (_matchingKvp.HasValue)
            {
                matchingKvp = _matchingKvp.Value;
                return true;
            }
            matchingKvp = default;
            return false;
        }

        internal static KeyValuePair<string, string>? GetByKeyIgnoreCase(this Dictionary<string, string> dictionary, string searchKey)
        {
            foreach (var kvp in dictionary)
            {
                if (string.Equals(kvp.Key, searchKey, StringComparison.CurrentCultureIgnoreCase))
                {
                    return kvp;
                }
            }
            return null;
        }
    }
}
