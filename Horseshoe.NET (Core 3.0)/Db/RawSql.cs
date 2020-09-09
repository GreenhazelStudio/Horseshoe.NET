using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public class RawSql
    {
        public string Expression { get; }

        public RawSql(string expression)
        {
            Expression = expression;
        }
    }
}
