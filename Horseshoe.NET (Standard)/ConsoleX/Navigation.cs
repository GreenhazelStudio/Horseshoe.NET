namespace Horseshoe.NET.ConsoleX
{
    public static class Navigation
    {
        public static void RestartRoutine()
        {
            throw new RestartRoutineException();
        }

        public static void ExitRoutine()
        {
            throw new ExitRoutineException();
        }

        public static void ExitApp()
        {
            throw new ExitAppException();
        }

        public static void CancelPassword()
        {
            throw new CancelPasswordException();
        }

        public class RestartRoutineException : BenignException
        {
            public RestartRoutineException() : base("Restarting routine...") { }
        }

        public class ExitRoutineException : BenignException
        {
            public ExitRoutineException() : base("Exiting routine...") { }
        }

        public class ExitAppException : BenignException
        {
            public bool ShowPrompt { get; set; }
            public ExitAppException(bool showPrompt = false) : base("Exiting application...") { ShowPrompt = showPrompt; }
        }

        public class CancelPasswordException : BenignException
        {
            public CancelPasswordException() : base("Cancelling password input...") { }
        }
    }
}
