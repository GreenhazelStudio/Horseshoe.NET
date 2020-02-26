using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Text;

namespace Horseshoe.NET.IO.FileImport
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Serialization irrelavent")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Only need certain constructors")]
    public class InvalidDatumException : DataImportException
    {
        public object Datum { get; set; }

        public string ColumnName { get; set; }

        public int Length { get; set; }

        public string DataRef { get; set; }

        public InvalidDatumException(string message, object datum, string columnName = null, int length = 0, string dataRef = null) : base(ToString(message, datum, columnName, length, dataRef))
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            DataRef = dataRef;
        }

        public InvalidDatumException(Exception innerException, object datum, string columnName = null, int length = 0, string dataRef = null) : base(ToString(innerException.Message, datum, columnName, length, dataRef), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            DataRef = dataRef;
        }

        public InvalidDatumException(string message, Exception innerException, object datum, string columnName = null, int length = 0, string dataRef = null) : base(ToString(message ?? innerException.Message, datum, columnName, length, dataRef), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            DataRef = dataRef;
        }

        static string ToString(string message, object datum, string columnName = null, int length = 0, string dataRef = null)
        {
            return
                (message != null ? message + " -- " : "") +
                "Datum = " + TextUtil.RevealNullOrBlank(datum).Trunc(24, truncPolicy: TruncatePolicy.LongEllipsis) +
                (columnName != null ? "; Column = " + columnName : "") +
                (length > 0 ? "; Length = " + length : "") +
                (dataRef != null ? "; Data Ref = " + dataRef : "");
        }
    }
}
