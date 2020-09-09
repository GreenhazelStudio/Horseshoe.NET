using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Collections.Extensions;

namespace Horseshoe.NET
{
    public class StringList : List<string>
    {
        public StringList() { }
        public StringList(IEnumerable<string> values) : base(values) { }

        public static implicit operator StringList(string value) => new StringList(value?.Split(',', ';', '|').ZapAndPrune());
        public static implicit operator StringList(string[] values) => new StringList(values?.ZapAndPrune());
    }
}
