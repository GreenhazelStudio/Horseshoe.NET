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

        public string Position { get; set; }

        public InvalidDatumException(string message, object datum, string columnName = null, int length = 0, string position = null) : base(ToMessageString(message, datum, columnName, length, position))
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            Position = position;
        }

        public InvalidDatumException(Exception innerException, object datum, string columnName = null, int length = 0, string position = null) : base(ToMessageString(innerException.Message, datum, columnName, length, position), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            Position = position;
        }

        public InvalidDatumException(string message, Exception innerException, object datum, string columnName = null, int length = 0, string position = null) : base(ToMessageString(message ?? innerException.Message, datum, columnName, length, position), innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            Position = position;
        }

        static string ToMessageString(string message, object datum, string columnName, int length, string position)
        {
            return
                (!string.IsNullOrEmpty(message) ? message + " -- " : "") +
                "{Value: \"" + TextUtil.RevealNullOrBlank(datum).Trunc(12, truncPolicy: TruncatePolicy.LongEllipsis) + "\"" + 
                (columnName != null ? "; Column: \"" + columnName : "\"") +
                (length > 0 ? "; Length: " + length : "") +
                (position != null ? "; Position: \"" + position : "\"") +
                "}";
        }
    }
}
