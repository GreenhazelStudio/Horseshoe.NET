using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;
using static Horseshoe.NET.Text.TextUtil;

namespace Horseshoe.NET.IO.FileImport
{
    public static class ImportUtil
    {
        //public static object ConvertDataElement(object value, Column column, string dataReference)
        //{
        //    if (value == null) return null;
        //    if (column?.ValueConverter == null)
        //    {
        //        return value;
        //    }
        //    try
        //    {
        //        return column.ValueConverter.Invoke(value);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new UtilityException(ex.Message + " -- occurred at " + dataReference + " -- value = " + (value.Equals("") ? "[blank]" : value), ex);
        //    }
        //}

        public static object ProcessDatum(object value, Column column, string dataReference)
        {
            if (column?.ValueConverter != null)
            {
                try
                {
                    value = column.ValueConverter.Invoke(value);
                }
                catch (Exception ex)
                {
                    throw new ImportedDatumException(ex, value, columnName: column?.Name, length: column.Width, dataRef: dataReference);
                }
            }
            if (value is string stringValue && column?.Width > 0)
            {
                if (stringValue.Length > column.Width)
                {
                    throw new ImportedDatumException("value exceeds max length", stringValue, columnName: column?.Name, length: column.Width, dataRef: dataReference);
                }
            }
            return value;
        }

        public static string PostParseString(string value, AutoTruncate autoTrunc)
        {
            if (autoTrunc == AutoTruncate.Zap)
            {
                value = Zap(value);
            }
            else if (autoTrunc == AutoTruncate.Trim)
            {
                value = value.Trim();
            }
            return value;
        }
    }
}
