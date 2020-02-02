using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Db.Internal
{
    public class SystemValue
    {
        public Func<DbProduct?, string, string> Formatter { get; }

        public string Expression { get; }

        SystemValue(Func<DbProduct?, string, string> formatter, string expression = null) 
        {
            Formatter = formatter;
            Expression = expression;
        }

        public static SystemValue Default { get; } = new SystemValue(_DefaultFormatter);

        public static SystemValue CurrentDate { get; } = new SystemValue(_CurrentDateFormatter);

        public static SystemValue Guid { get; } = new SystemValue(_GuidFormatter);

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
                    return "CURRENT_DATE_COLUMN_REQUIRES_A_SPECIFIC_VENDOR";
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
                    return "GUID_COLUMN_REQUIRES_A_SPECIFIC_VENDOR";
            }
        };

        static Func<DbProduct?, string, string> _ExpressionFormatter => (product, expression) =>
        {
            return expression;
        };
    }
}
