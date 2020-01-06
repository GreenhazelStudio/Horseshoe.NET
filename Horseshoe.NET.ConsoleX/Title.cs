using System;
using System.Text;

namespace Horseshoe.NET.ConsoleX
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Only used in console display, by convention, never compared")]
    public struct Title
    {
        public string Text { get; }
        public string Xtra { get; }

        public Title(string text, string xtra = null)
        {
            Text = text ?? throw new ValidationException("Title text may not be null");
            Xtra = xtra;
        }

        public static implicit operator Title(string text) => new Title(text);

        public override string ToString()
        {
            return Render();
        }

        public string Render()
        {
            if (Xtra == null) return Text;
            return Text + " - " + Xtra;
        }
    }
}
