using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Objects;

namespace Horseshoe.NET.ConsoleX
{
    public abstract class ConsoleApp
    {
        public virtual string SplashMessage { get; }

        public virtual IEnumerable<string> SplashMessageLines { get; }

        public virtual Title MainMenuTitle { get; } = "Main Menu";

        public virtual IEnumerable<Routine> MainMenu { get; }

        public virtual bool DisplayMainMenuRoutineTitles { get; } = true;

        public virtual Action<MenuSelection<Routine>> OnMainMenuSelection { get; }

        public virtual bool Looping { get; } = true;

        public virtual int SpacesAfterLoop { get; } = 1;

        public virtual LoopingPolicy LoopingPolicy { get; }

        public virtual bool DisplayExceptionsRecursively { get; }

        private bool ApplicationExited { get; set; }

        public virtual void Run()
        {
            if (MainMenu == null)
            {
                throw new UtilityException("ConsoleApp requires one or more of the following overrides: IEnumerable<Routine> MainMenu { get; }, void Run()");
            }
            if ((LoopingPolicy & LoopingPolicy.RenderSplash) != LoopingPolicy.RenderSplash)
            {
                if (SplashMessageLines != null) RenderSplash(SplashMessageLines);
                else if (SplashMessage != null) RenderSplash(SplashMessage);
            }
            _RunImpl();
            while (Looping && !ApplicationExited)
            {
                if ((LoopingPolicy & LoopingPolicy.ClearScreen) == LoopingPolicy.ClearScreen)
                {
                    Console.Clear();
                }
                else
                {
                    for (int i = 0; i < SpacesAfterLoop; i++)
                    {
                        Console.WriteLine();
                    }
                }
                _RunImpl();
            }
        }

        private void _RunImpl()
        {
            try
            {
                if ((LoopingPolicy & LoopingPolicy.RenderSplash) == LoopingPolicy.RenderSplash)
                {
                    if (SplashMessageLines != null) RenderSplash(SplashMessageLines);
                    else if (SplashMessage != null) RenderSplash(SplashMessage);
                }
                PromptMenu
                (
                    MainMenu,
                    title: MainMenuTitle,
                    onMenuSelection: OnMainMenuSelection
                );
            }
            catch (Navigation.ExitAppException ex)   // exit gracefully
            {
                ApplicationExited = true;
                if (ex.ShowPrompt)
                {
                    PromptExit(padBefore: 2);
                }
            }
            catch (Exception ex)
            {
                RenderException(ex, recursive: DisplayExceptionsRecursively, padBefore: 1);
                if (!Looping)
                {
                    PromptExit(padBefore: 2);
                }
            }
        }

        public static void PromptContinue(int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.PromptContinue(padBefore: padBefore, padAfter: padAfter);
        }

        public static void PromptContinue(string prompt, int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.PromptContinue(prompt, padBefore: padBefore, padAfter: padAfter);
        }

        public static void PromptExit(int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.PromptContinue(padBefore: padBefore, padAfter: padAfter);
        }

        public static void PromptExit(string prompt, int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.PromptContinue(prompt, padBefore: padBefore, padAfter: padAfter);
        }

        public static string PromptInput(int padBefore = 0, int padAfter = 1, bool allowExitApplication = false)
        {
            return ConsoleUtil.PromptInput(padBefore: padBefore, padAfter: padAfter, allowExitApplication: allowExitApplication);
        }

        public static string PromptInput(string prompt, int padBefore = 0, int padAfter = 1, bool allowExitApplication = false)
        {
            return ConsoleUtil.PromptInput(prompt, padBefore: padBefore, padAfter: padAfter, allowExitApplication: allowExitApplication);
        }

        public static string PromptPassword(int padBefore = 0, int padAfter = 1)
        {
            return ConsoleUtil.PromptPassword(padBefore: padBefore, padAfter: padAfter);
        }

        public static string PromptPassword(string prompt, int padBefore = 0, int padAfter = 1)
        {
            return ConsoleUtil.PromptPassword(prompt, padBefore: padBefore, padAfter: padAfter);
        }

        public static SecureString PromptPasswordSecure(int padBefore = 0, int padAfter = 1)
        {
            return ConsoleUtil.PromptPasswordSecure(padBefore: padBefore, padAfter: padAfter);
        }

        public static SecureString PromptPasswordSecure(string prompt, int padBefore = 0, int padAfter = 1)
        {
            return ConsoleUtil.PromptPasswordSecure(prompt, padBefore: padBefore, padAfter: padAfter);
        }

        public static MenuSelection<E> PromptMenu<E>
        (
            IEnumerable<E> menuItems,
            IEnumerable<Routine> customItemsToPrepend = null,
            IEnumerable<Routine> customItemsToAppend = null,
            Func<E, string> renderer = null,
            Title? title = null,
            string prompt = ">",
            int padBefore = 0,
            int padAfter = 1,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            bool allowExitApplication = true,
            Action<MenuSelection<E>> onMenuSelection = null
        ) where E : class
        {
            return ConsoleUtil.PromptMenu
            (
                menuItems,
                customItemsToPrepend: customItemsToPrepend,
                customItemsToAppend: customItemsToAppend,
                renderer: renderer,
                title: title,
                prompt: prompt,
                padAfter: padAfter,
                padBefore: padBefore,
                allowArbitraryInput: allowArbitraryInput,
                allowMultipleSelection: allowMultipleSelection,
                allowExitApplication: allowExitApplication,
                onMenuSelection: onMenuSelection
            );
        }

        /// <summary>
        /// Search the calling assembly for subclasses of Routine and instantiate an alphabetized array
        /// </summary>
        /// <param name="matchBaseNamespace">Filter out routines in child and unrelated namespaces</param>
        /// <param name="namespaceToMatch">Select routines only in this namespace, if provided</param>
        /// <returns></returns>
        public static IEnumerable<Routine> FindRoutines(bool matchBaseNamespace = false, string namespaceToMatch = null)
        {
            var assembly = Assembly.GetCallingAssembly();
            var routineTypes = assembly.GetTypes()
                .Where(t => 
                    t.IsSubclassOf(typeof(Routine)) && 
                    (!matchBaseNamespace || Equals(t.Namespace, assembly.GetName().Name)) &&
                    (namespaceToMatch == null || Equals(t.Namespace, namespaceToMatch))
                )
                .OrderBy(t => t.Name);
            var array = routineTypes
                .Select(t => (Routine)ObjectUtil.GetInstance(t))
                .ToArray();
            return array;
        }

        public static void RenderAlert(string message, int padBefore = 1, int padAfter = 1)
        {
            ConsoleUtil.RenderAlert(message, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderException(Exception ex, bool displayShortName = false, bool displayMessage = true, bool displayStackTrace = true, bool recursive = false, int padBefore = 0, int padAfter = 1)
        {
            ConsoleUtil.RenderException(ex, displayShortName: displayShortName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, recursive: recursive, padBefore: padBefore, padAfter: padAfter);
        }

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
            ConsoleUtil.RenderList
            (
                list,
                indexPolicy: indexPolicy,
                renderer: renderer,
                title: title,
                padBefore: padBefore,
                padAfter: padAfter
            );
        }

        public static void RenderListTitle(Title title, int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.RenderListTitle(title, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderMenuTitle(Title title, int padBefore = 0, int padAfter = 0)
        {
            ConsoleUtil.RenderMenuTitle(title, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderSplash(string splashMessage, int padBefore = 1, int padAfter = 2)
        {
            RenderSplash(new[] { splashMessage }, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderSplash(IEnumerable<string> splashMessageLines, int padBefore = 1, int padAfter = 2)
        {
            ConsoleUtil.RenderSplash(splashMessageLines, padBefore: padBefore, padAfter: padAfter);
        }

        public static void StartConsoleApp(ConsoleApp app)
        {
            app.Run();
        }

        public static void StartConsoleApp<T>() where T : ConsoleApp, new()
        {
            ((ConsoleApp)ObjectUtil.GetInstance<T>()).Run();
        }
    }
}
