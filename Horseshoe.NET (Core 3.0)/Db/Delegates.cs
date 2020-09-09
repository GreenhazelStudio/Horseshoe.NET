using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public delegate void UsingConnectionString(string connectionString, string source = null);
    public delegate void UsingCredentials(string userName, string passwordDescription, string source = null);
    public delegate void UsingStatement(string statement);
    public delegate void ColumnSearchedByValue(string column);
}
