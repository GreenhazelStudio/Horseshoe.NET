using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Text
{
    public delegate void TextCleanCharacterIdentified(char charIdentified, string replacement, int position, string category, bool isCustomDictionary);
}
