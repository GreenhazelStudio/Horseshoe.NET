using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.IO.FileImport;

namespace Horseshoe.NET.Excel
{
    public class ExcelColumn
    {
        public static Column ExcelDate(string name, int width = 0) => new Column
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = (obj) => ExcelUtil.ConvertToDateTime(obj, suppressErrors: false),
            Format = Column.DateFormatNoMilliseconds
        };

        public static Column ExcelNDate(string name, int width = 0) => new Column
        {
            Name = name,
            Width = width,
            DataType = DataType.DateTime,
            ValueConverter = (obj) => ExcelUtil.ConvertToNDateTime(obj, suppressErrors: false),
            Format = Column.DateFormatNoMilliseconds
        };
    }
}
