using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.FileImport.Enums
{
    [Flags]
    public enum DataErrorHandlingPolicy
    {
        Ignore = 0,
        Throw = 1,
        Embed = 2,
        Announce = 4
    }
}
