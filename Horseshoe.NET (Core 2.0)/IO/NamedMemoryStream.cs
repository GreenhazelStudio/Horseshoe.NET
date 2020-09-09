using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO
{
    public class NamedMemoryStream : MemoryStream
    {
        public string Name { get; set; }

        public NamedMemoryStream()
        {
        }

        public NamedMemoryStream(string name)
        {
            Name = name;
        }
    }
}
