using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db
{
    public class SystemValue : RawSql
    {
        public Func<DbProduct?, string, string> Formatter { get; }

        SystemValue(Func<DbProduct?, string, string> formatter, string expression) : base(expression)
        {
            Formatter = formatter;
        }

        public static SystemValue Default { get; } = new SystemValue(_DefaultFormatter, "_DEFAULT_");

        public static SystemValue CurrentDate { get; } = new SystemValue(_CurrentDateFormatter, "_CURRENT_DATE_");

        public static SystemValue Guid { get; } = new SystemValue(_GuidFormatter, "_GUID_");

        public static SystemValue UseExpression(string expression) => new SystemValue(_ExpressionFormatter, expression: expression);

        static Func<DbProduct?, string, string> _DefaultFormatter => (product, expression) =>
        {
            return "DEFAULT";
        };

        static Func<DbProduct?, string, string> _CurrentDateFormatter => (product, expression) =>
        {
            switch (product)
            {
                case DbProduct.SqlServer:
                    return "GETDATE()";
                case DbProduct.Oracle:
                    return "SYSDATE";
                default:
                    if (product.HasValue)
                    {
                        throw new UtilityException("Horseshoe.NET.Db.SystemValue has no " + product + " formatter for " + expression);
                    }
                    else
                    {
                        throw new UtilityException("Horseshoe.NET.Db.SystemValue requires the DbProduct (e.g. Oracle, SqlServer, etc.) so it can supply a formatter for " + expression);
                    }
            }
        };

        static Func<DbProduct?, string, string> _GuidFormatter => (product, expression) =>
        {
            switch (product)
            {
                case DbProduct.SqlServer:
                    return "NEWID()";
                case DbProduct.Oracle:
                    return "SYS_GUID()";
                default:
                    if (product.HasValue)
                    {
                        throw new UtilityException("Horseshoe.NET.Db.SystemValue has no " + product + " formatter for " + expression);
                    }
                    else
                    {
                        throw new UtilityException("Horseshoe.NET.Db.SystemValue requires the DbProduct (e.g. Oracle, SqlServer, etc.) so it can supply a formatter for " + expression);
                    }
            }
        };

        static Func<DbProduct?, string, string> _ExpressionFormatter => (product, expression) =>
        {
            return expression;
        };
    }
}
