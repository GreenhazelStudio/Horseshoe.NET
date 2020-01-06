using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Horseshoe.NET.DataAccess.Internal;
using static Horseshoe.NET.DataAccess.DataUtil;

namespace Horseshoe.NET.DataAccess
{
    public class Column
    {
        public string Name { get; }
        public object Value { get; }
        public DbProduct? Vendor { get; }

        public Column(string name, object value = null, DbProduct? product = null)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Name cannot be null or blank");
            }
            Name = name;
            Value = value;
            Vendor = product;
        }

        public static Column Default(string name) => new Column(name, value: SystemValue.Default);

        public static Column CurrentDate(string name, DbProduct? product = null) => new Column(name, value: SystemValue.CurrentDate, product: product);

        public static Column Guid(string name) => new Column(name, value: SystemValue.Guid);

        public static Column UseExpression(string name, string expression) => new Column(name, value: SystemValue.UseExpression(expression));

        public override string ToString()
        {
            return Name;
        }

        public string ToString(DbProduct? product)
        {
            return RenderColumnName(Name, product);
        }

        public string ToDMLString(DbProduct? product = null)
        {
            return ToString(product) + " = " + Sqlize(Value, Vendor ?? product);
        }
    }
}
