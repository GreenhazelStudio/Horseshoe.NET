using System;
using System.Collections.Generic;
using System.Security;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ConsoleX
{
    public abstract class Routine
    {
        public virtual Title Title => GetType().FullName;

        public virtual bool RenderTitleOnRun { get; } = true;

        public virtual Title MenuTitle => Title + " Menu";

        public virtual IEnumerable<Routine> Menu { get; }

        public virtual bool AutoAppendExitRoutineMenuItem { get; } = true;

        public virtual bool AutoAppendRestartRoutineMenuItem { get; }

        public virtual Action<MenuSelection<Routine>> OnMenuSelection { get; }

        public virtual Action Action { get; }

        public virtual string Command { get; }

        public virtual bool ContinuousDisplay { get; } = true;

        public virtual bool Looping { get; }

        public virtual LoopingPolicy LoopingPolicy { get; }

        public virtual int SpacesAfterLoop { get; } = 1;

        private bool RoutineRestarted { get; set; }

        private bool RoutineExited { get; set; }

        public virtual void Run()
        {
            if ((Menu == null && Action == null) || (Menu != null && Action != null))
            {
                throw new UtilityException("Routines must override Run() or exactly one of the following properties: Action, Menu");
            }
            RoutineExited = false;
            bool firstRun = true;
            while ((Looping || RoutineRestarted || firstRun) && !RoutineExited)
            {
                RoutineRestarted = false;
                firstRun = false;
                if ((LoopingPolicy & LoopingPolicy.ClearScreen) == LoopingPolicy.ClearScreen)
                {
                    Console.Clear();
                }
                else if (!firstRun)
                {
                    for (int i = 0; i < SpacesAfterLoop; i++)
                    {
                        Console.WriteLine();
                    }
                }
                if (RenderTitleOnRun)
                {
                    RenderRoutineTitle();
                }
                _RunImpl();
            }
        }

        private void _RunImpl()
        {
            try
            {
                if (Action != null)
                {
                    Action.Invoke();
                }
                if (Menu != null)
                {
                    PromptMenu
                    (
                        Menu, 
                        title: MenuTitle,
                        autoAppendExitRoutineMenuItem: true,
                        onMenuSelection: OnMenuSelection
                    );
                }
            }
            catch (Navigation.RestartRoutineException)
            {
                RoutineRestarted = true;
            }
            catch (Navigation.ExitRoutineException)
            {
                RoutineExited = true;
            }
        }

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
            bool autoAppendRestartRoutineMenuItem = false,
            bool autoAppendExitRoutineMenuItem = true,
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
                customItemsToAppend: BuildCustomMenuItems(customItemsToAppend, autoAppendRestartRoutineMenuItem, autoAppendExitRoutineMenuItem),
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

        private static IEnumerable<Routine> BuildCustomMenuItems
        (
            IEnumerable<Routine> customItemsToAppend, 
            bool autoAppendRestartRoutineMenuItem,
            bool autoAppendExitRoutineMenuItem
        )
        {
            customItemsToAppend = CollectionUtil.ConcatIf(autoAppendRestartRoutineMenuItem, customItemsToAppend, CreateRestartRoutineMenuItem());
            customItemsToAppend = CollectionUtil.ConcatIf(autoAppendExitRoutineMenuItem, customItemsToAppend, CreateExitRoutineMenuItem());
            return customItemsToAppend;
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

        public void RenderRoutineTitle(int padBefore = 1, int padAfter = 1)
        {
            ConsoleUtil.RenderRoutineTitle(Title, padBefore: padBefore, padAfter: padAfter);
        }

        public static void RenderRoutineTitle(Title title, int padBefore = 1, int padAfter = 1)
        {
            ConsoleUtil.RenderRoutineTitle(title, padBefore: padBefore, padAfter: padAfter);
        }

        protected static Routine CreateRestartRoutineMenuItem(string command = "R", string text = "Restart", Action beforeRestart = null)
        {
            return BuildCustom
            (
                text,
                () =>
                {
                    beforeRestart?.Invoke();
                    Restart();  // restart routine
                },
                command: command
            );
        }

        protected static Routine CreateExitRoutineMenuItem(string command = "/", string text = "Go Back", Action beforeExit = null)
        {
            return BuildCustom
            (
                text,
                () =>
                {
                    beforeExit?.Invoke();
                    Exit();  // exit routine
                },
                command: command
            );
        }

        public override string ToString()
        {
            return Title;
        }

        public static Routine Build(string title, Action action)
        {
            return new _BuildImpl(title, action);
        }

        public static Routine BuildCustom(string title, Action action, string command = "")
        {
            return new _BuildImpl(title, action, command: command);
        }

        internal class _BuildImpl : Routine
        {
            public override Title Title { get; }

            public override bool RenderTitleOnRun => false;

            public override Action Action { get; }

            public override string Command { get; }

            public override bool Looping => false;  // redundant

            internal _BuildImpl(string title, Action action, string command = null)
            {
                Title = title;
                Action = action;
                Command = command;
            }
        }
    }
}
