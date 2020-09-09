using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Collections
{
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Not applicable to benign exceptions")]
    public class IterationException : BenignException
    {
        public bool Break { get; set; }
        public bool Continue { get; set; }
    }
}
