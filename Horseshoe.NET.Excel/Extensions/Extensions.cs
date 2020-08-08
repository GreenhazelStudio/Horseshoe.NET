using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Horseshoe.NET.ObjectClean.Methods;

using NPOI.SS.UserModel;

namespace Horseshoe.NET.Excel.Extensions
{
    internal static class Extensions
    {
        internal static bool IsBlank(this IRow row)
        {
            foreach (var cell in row.Cells)
            {
                if (!IsBlank(cell)) return false;
            }
            return true;
        }

        static bool IsBlank(ICell cell)
        {
            if (cell.CellType == CellType.Blank) return true;
            var cellType = cell.CellType == CellType.Formula
                ? cell.CachedFormulaResultType
                : cell.CellType;
            return cellType == CellType.String && ZapString(cell.StringCellValue) == null;
        }
    }
}
