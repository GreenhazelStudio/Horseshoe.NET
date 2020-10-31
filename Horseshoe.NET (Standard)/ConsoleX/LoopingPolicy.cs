using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.ConsoleX
{
    [Flags]
    public enum LoopingPolicy
    {
        NoAction,
        ClearScreen,
        RenderSplash
    }
}
