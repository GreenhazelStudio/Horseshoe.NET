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
    public class ImportedDatumException : Exception
    {
        public object Datum { get; set; }

        public string ColumnName { get; set; }

        public int Length { get; set; }

        public string DataRef { get; set; }

        public ImportedDatumException(object datum, string message, string columnName = null, int length = 0, string dataRef = null) : base(message)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            DataRef = dataRef;
        }

        public ImportedDatumException(object datum, string message, Exception innerException, string columnName = null, int length = 0, string dataRef = null) : base(message, innerException)
        {
            Datum = datum;
            ColumnName = columnName;
            Length = length;
            DataRef = dataRef;
        }

        public override string ToString()
        {
            return
                "datum=" + TextUtil.RevealNullOrBlank(Datum).Trunc(20, truncPolicy: TruncatePolicy.LongEllipsis) +
                (ColumnName != null ? "; col=" + ColumnName : "") +
                (Length > 0 ? "; len=" + Length : "") +
                (DataRef != null ? "; dataref=" + DataRef : "");
        }
    }
}
