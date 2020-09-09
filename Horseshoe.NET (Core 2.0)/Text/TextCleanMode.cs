using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Text
{
    [Flags]
    public enum TextCleanMode
    {
        None = 0,
        RemoveWhitespace = 1,
        NormalizeWhitespace = 2,
        NormalizeAndCombineWhitespace = 4,
        RemoveNonprintables = 8,
        NormalizePunctuation = 16,
        NormalizeNumbersMathAndProgramming = 32,
        NormalizeSuperscriptsAndSubscripts = 64,
        NormalizeLatin = 128,
        NormalizeLatinExtended = 256,
        NormalizeGreek = 512,
        NormalizeGreekExtended = 1024,
        NormalizeCyrillic = 2048,
        //NormalizeCyrillicExtended = 4096,
        NormalizeSymbols = 8192,
        All = 16384,
        AllExtended = 32768
    }
}
