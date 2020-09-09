using System;
using System.Collections.Generic;

namespace Horseshoe.NET.ConsoleX
{
    public class CustomMenuItem : IEquatable<CustomMenuItem>
    {
        public string Command { get; set; } = "";
        public string Text { get; set; }
        public bool PrependToMenu { get; set; }
        public Action Action { get; set; }
        public bool Inert { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is CustomMenuItem other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(CustomMenuItem other)
        {
            return other != null &&
                   Command == other.Command &&
                   Text == other.Text &&
                   PrependToMenu == other.PrependToMenu;
        }

        public override int GetHashCode()
        {
            var hashCode = 1259522663;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Command);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = hashCode * -1521134295 + PrependToMenu.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CustomMenuItem left, CustomMenuItem right)
        {
            return EqualityComparer<CustomMenuItem>.Default.Equals(left, right);
        }

        public static bool operator !=(CustomMenuItem left, CustomMenuItem right)
        {
            return !(left == right);
        }
    }
}
