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
    public abstract class ConsoleApp<T> where T : ConsoleApp<T>, new()
    {
        public virtual bool Looping => true;

        public abstract void Run();

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
            return ConsoleUtil.PromptMenu
            (
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
            return ConsoleUtil.PromptMenu
            (
                menuItems,
                renderer: renderer,
                title: title,
                customMenuItems: customMenuItems,
                prompt: prompt,
                padAfter: padAfter,
                padBefore: padBefore,
                allowArbitraryInput: allowArbitraryInput,
                allowMultipleSelection: allowMultipleSelection,
                allowExitApplication: allowExitApplication
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
                    typeof(Routine).IsAssignableFrom(t) && 
                    (!matchBaseNamespace || Equals(t.Namespace, assembly.GetName().Name)) &&
                    (namespaceToMatch == null || Equals(t.Namespace, namespaceToMatch))
                )
                .OrderBy(t => t.Name);
            var array = routineTypes
                .Select(t => (Routine)ObjectUtil.GetInstance(t))
                .ToArray();
            return array;
        }

        public static MenuSelection<Routine> PromptRoutineMenu
        (
            IEnumerable<Routine> menu,
            int padBefore = 0,
            Title? title = null,
            int padAfter = 1,
            string prompt = ">",
            bool autoRun = false
        )
        {
            var routineSelection = PromptMenu
            (
                menu,
                padBefore: padBefore,
                title: title,
                padAfter: padAfter,
                prompt: prompt
            );

            if (autoRun)
            {
                routineSelection.SelectedItem.Run();
            }
            return routineSelection;
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

        public static void RenderSplash(string welcomeMessage, int padBefore = 1, int padAfter = 2)
        {
            ConsoleUtil.RenderSplash(welcomeMessage, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderSplash(string[] welcomeMessageLines, int padBefore = 1, int padAfter = 2)
        {
            ConsoleUtil.RenderSplash(welcomeMessageLines, padBefore: padBefore, padAfter: padAfter);
        }

        static T App { get; } = new T();

        public static void StartApp()
        {
            try
            {
                Console.Clear();
                App.Run();
                if (App.Looping)
                {
                    StartApp();
                }
            }
            catch (Navigation.ExitAppException ex)
            {
                // exit normally (benign exception)
                if (ex.ShowPrompt)
                {
                    PromptExit();
                }
            }
            catch (Exception ex)
            {
                RenderException(ex, padBefore: 1);
                PromptExit();
            }
        }
    }
}
