using System;
using System.Diagnostics.CodeAnalysis;

namespace Horseshoe.NET
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Used only for control flow and message delivery, by convention")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Used only for control flow and message delivery, by convention")]
    public class BenignException : Exception
    {
        public BenignException() : this("(control flow / message delivery)") { }
        public BenignException(string message) : base(message) { }
    }
}
