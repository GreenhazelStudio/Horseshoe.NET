using System;
using System.Collections.Generic;
using System.Security;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ConsoleX
{
    public abstract class Routine
    {
        public virtual Title Title => GetType().FullName;

        public virtual bool ContinuousDisplay => true;

        public virtual bool Looping => false;

        public void Run(bool clearScreen = true)
        {
            if (clearScreen)
            {
                Console.Clear();
                RenderRoutineTitle();
            }
            try
            {
                Do();
                if (Looping)
                {
                    if (ContinuousDisplay)
                    {
                        Console.WriteLine();
                    }
                    Run(!ContinuousDisplay);
                }
            }
            catch (Navigation.RestartRoutineException)
            {
                // restart routine (benign exception)
                Run();
            }
            catch (Navigation.ExitRoutineException)
            {
                // exit routine (benign exception)
            }
        }

        public abstract void Do();

        protected static void Restart()
        {
            Navigation.RestartRoutine();
        }

        protected static void Exit()
        {
            Navigation.ExitRoutine();
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

        public static MenuSelection<object> PromptMenu
        (
            Title? title = null,
            IEnumerable<CustomMenuItem> customMenuItems = null,
            string prompt = ">",
            int padBefore = 0,
            int padAfter = 1,
            bool autoAppendExitRoutineMenuItem = true,
            bool allowArbitraryInput = false,
            bool allowMultipleSelection = false,
            bool allowExitApplication = true
        )
        {
            return ConsoleUtil.PromptMenu
            (
                title: title,
                customMenuItems: autoAppendExitRoutineMenuItem
                    ? CollectionUtil.ConcatIf(customMenuItems, new[] { CreateExitRoutineMenuItem() })
                    : customMenuItems,
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
            bool autoAppendExitRoutineMenuItem = true,
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
                customMenuItems: autoAppendExitRoutineMenuItem 
                    ? CollectionUtil.ConcatIf(customMenuItems, new[] { CreateExitRoutineMenuItem() })
                    : customMenuItems,
                prompt: prompt,
                padAfter: padAfter,
                padBefore: padBefore,
                allowArbitraryInput: allowArbitraryInput,
                allowMultipleSelection: allowMultipleSelection,
                allowExitApplication: allowExitApplication
            );
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

        public static void RenderException(Exception ex, bool displayFullClassName = false, bool displayMessage = true, bool displayStackTrace = false, bool recursive = false, int padBefore = 0, int padAfter = 1)
        {
            ConsoleUtil.RenderException(ex, displayFullClassName: displayFullClassName, displayMessage: displayMessage, displayStackTrace: displayStackTrace, recursive: recursive, padBefore: padBefore, padAfter: padAfter);
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

        public void RenderRoutineTitle(int padBefore = 1, int padAfter = 1)
        {
            ConsoleUtil.RenderRoutineTitle(Title, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderRoutineTitle(Title title, int padBefore = 1, int padAfter = 1)
        {
            ConsoleUtil.RenderRoutineTitle(title, padBefore: padBefore, padAfter: padAfter);
        }

        protected static CustomMenuItem CreateRestartRoutineMenuItem(string command = "R", string text = "Restart", bool prependToMenu = false, Action beforeRestart = null)
        {
            return new CustomMenuItem
            {
                Command = command,
                Text = text,
                Action = () =>
                {
                    beforeRestart?.Invoke();
                    Restart();
                },
                PrependToMenu = prependToMenu
            };
        }

        protected static CustomMenuItem CreateExitRoutineMenuItem(string command = "/", string text = "Go Back", bool prependToMenu = false, Action beforeExit = null)
        {
            return new CustomMenuItem
            {
                Command = command,
                Text = text,
                Action = () =>
                {
                    beforeExit?.Invoke();
                    Exit();
                },
                PrependToMenu = prependToMenu
            };
        }

        public override string ToString()
        {
            return Title.ToString();
        }
    }
}
