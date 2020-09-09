using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Text
{
    public class TextCleanRules
    {
        public TextCleanMode Mode { get; set; }

        private object _customMap;
        public object CustomMap 
        { 
            get { return _customMap; }
            set
            {
                if (value != null && !(value is IDictionary<char, char[]> || value is IDictionary<string, char[]>))
                {
                    throw new UtilityException("a custom map must be either of type IDictionary<char, char[]> or IDictionary<string, char[]>");
                }
                _customMap = value;
            }
        }

        public IEnumerable<char> CharsToRemove { get; set; }

        public TextCleanRules() 
        {
            Mode = TextCleanMode.None;
        }

        public TextCleanRules(TextCleanMode mode) 
        {
            Mode = mode;
        }
    }
}
