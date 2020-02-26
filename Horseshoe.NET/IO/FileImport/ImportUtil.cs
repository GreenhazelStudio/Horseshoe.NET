using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.IO.FileImport.Enums;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileImport
{
    public static class ImportUtil
    {
        public static event Action<DataImportException> DataImportErrorOccurred;

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

        public static object ProcessDatum(object value, Column column, string dataReference, DataErrorHandlingPolicy dataErrorHandling)
        {
            DataImportException diex = null;
            if (column?.ValueConverter != null)
            {
                try
                {
                    value = column.ValueConverter.Invoke(value);
                }
                catch (Exception ex)
                {
                    diex = new InvalidDatumException(ex, value, columnName: column?.Name, length: column.Width, dataRef: dataReference);
                }
            }
            if (value is string stringValue && column?.Width > 0 && stringValue.Length > column.Width)
            {
                diex = new InvalidDatumException("value exceeds max length", stringValue, columnName: column?.Name, length: column.Width, dataRef: dataReference);
            }
            if (diex != null)
            {
                if ((dataErrorHandling & DataErrorHandlingPolicy.Announce) == DataErrorHandlingPolicy.Announce)
                {
                    if (DataImportErrorOccurred == null)
                    {
                        throw new UtilityException("Error could not be announced due to no listener was supplied (e.g. ImportUtil.DataImportErrorOccurred += (diex) => { ... })");
                    }
                    DataImportErrorOccurred.Invoke(diex);
                }
                if ((dataErrorHandling & DataErrorHandlingPolicy.Embed) == DataErrorHandlingPolicy.Embed)
                {
                    value = diex;
                }
                if ((dataErrorHandling & DataErrorHandlingPolicy.Throw) == DataErrorHandlingPolicy.Throw)
                {
                    throw diex;
                }
            }
            return value;
        }

        public static string PostParseString(string value, AutoTruncate autoTrunc)
        {
            if (autoTrunc == AutoTruncate.Zap)
            {
                value = TextUtil.Zap(value);
            }
            else if (autoTrunc == AutoTruncate.Trim)
            {
                value = value.Trim();
            }
            return value;
        }
    }
}
