using System;

namespace Horseshoe.NET.ConsoleX.Extensions
{
    public static class Extensions
    {
        public static bool IsLetter(this ConsoleKeyInfo info)
        {
            return char.IsLetter(info.KeyChar);
        }

        public static bool IsNumber(this ConsoleKeyInfo info)
        {
            return char.IsNumber(info.KeyChar);
        }

        public static bool IsLetterOrDigit(this ConsoleKeyInfo info)
        {
            return char.IsLetterOrDigit(info.KeyChar);
        }

        public static bool IsSpace(this ConsoleKeyInfo info)
        {
            return info.KeyChar == ' ';
        }

        public static bool IsSpecialCharacter(this ConsoleKeyInfo info)
        {
            switch (info.KeyChar)
            {
                case '`':
                case '~':
                case '@':
                case '#':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '_':
                case '=':
                case '+':
                case '\\':
                case '|':
                case '<':
                case '>':
                case '/':
                    return true;
            }
            return false;
        }

        public static bool IsPunctuation(this ConsoleKeyInfo info)
        {
            switch (info.KeyChar)
            {
                case '.':
                case '?':
                case '!':
                case ',':
                case ';':
                case ':':
                case '-':
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '\'':
                case '"':
                    return true;
            }
            return false;
        }

        public static bool IsCursorNavigation(this ConsoleKeyInfo info)
        {
            switch (info.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.End:
                case ConsoleKey.Home:
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.Tab:
                case ConsoleKey.Enter:
                case ConsoleKey.Backspace:
                case ConsoleKey.Delete:
                case ConsoleKey.Insert:
                    return true;
            }
            return false;
        }
    }
}
