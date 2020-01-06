using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horseshoe.NET.IO.FileImport
{
    public enum BlankRowMode
    {
        Allow,
        Skip,
        StopProcessing,
        ThrowException
    }
}
