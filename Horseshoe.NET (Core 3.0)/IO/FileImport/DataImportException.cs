using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.FileImport
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Serialization irrelavent")]
    public class DataImportException : Exception
    {
        public DataImportException()
        {
        }

        public DataImportException(string message) : base(message)
        {
        }

        public DataImportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
