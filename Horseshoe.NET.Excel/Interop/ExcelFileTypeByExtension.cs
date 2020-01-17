using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Office.Interop.Excel;

namespace Horseshoe.NET.Excel.Interop
{
    public enum ExcelFileTypeByExtension
    {
        Xls = XlFileFormat.xlExcel8,
        Xlt = XlFileFormat.xlTemplate,
        Xlsx = XlFileFormat.xlOpenXMLWorkbook,
        Xltx = XlFileFormat.xlOpenXMLTemplate,
        Xlsm = XlFileFormat.xlOpenXMLWorkbookMacroEnabled,
        Xlsb = XlFileFormat.xlExcel12,
        Csv = XlFileFormat.xlCSV
    }
}
