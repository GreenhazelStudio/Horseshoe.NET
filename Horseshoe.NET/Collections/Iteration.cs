using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Collections
{
    public sealed class Iteration
    {
        public static void Exit()
        {
            throw new IterationException { Break = true };
        }

        public static void Next()
        {
            throw new IterationException { Continue = true };
        }
    }
}
