using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Only basic usage is expected, serialization is irrelevant")]
    public class ValidationException : Exception
    {
        public override string Message
        {
            get
            {
                return TextUtil.Crop
                (
                    base.Message +
                    (
                        ValidationMessages == null || !ValidationMessages.Any()
                            ? "" 
                            : " (x" + ValidationMessages.Count() + "): " + string.Join(";", ValidationMessages)
                    ),
                    75,
                    truncateMarker: TruncateMarker.LongEllipsis
                );
            }
        }

        public string ValidationMessage
        {
            set { ValidationMessages = new string[] { value }; }
        }

        public string[] ValidationMessages { get; set; }

        public bool HasValidationMessages =>  ValidationMessages?.Any() ?? false;

        public ValidationException() : base("Validation failed") { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
        public ValidationException(Exception innerException) : base("Validation failed", innerException) { }
    }
}
