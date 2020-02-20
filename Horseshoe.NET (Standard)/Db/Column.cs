using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static Horseshoe.NET.Db.DataUtil;

namespace Horseshoe.NET.Db
{
    public class Column
    {
        public string Name { get; }
        public object Value { get; }
        public DbProduct? Product { get; set; }

        public Column(string name, object value = null, DbProduct? product = null)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Name cannot be null or blank");
            }
            Name = name;
            Value = value;
            Product = product;
        }

        public override string ToString()
        {
            return RenderColumnName(Name, Product);
        }

        public string ToDMLString()
        {
            return ToString() + " = " + Sqlize(Value, Product);
        }
    }
}
