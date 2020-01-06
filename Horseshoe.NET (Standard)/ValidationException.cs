using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.Text;

namespace Horseshoe.NET
{
    public class ValidationException : Exception
    {
        public override string Message
        {
            get
            {
                return TextUtil.Trunc
                (
                    base.Message +
                    (
                        ValidationMessages == null || !ValidationMessages.Any()
                            ? "" 
                            : " (x" + ValidationMessages.Count() + "): " + string.Join(";", ValidationMessages)
                    ),
                    75,
                    truncPolicy: TruncatePolicy.Ellipsis
                );
            }
        }

        public string ValidationMessage
        {
            set { ValidationMessages = new string[] { value }; }
        }

        public string[] ValidationMessages { get; set; }

        public bool HasValidationMessages =>  ValidationMessages?.Any() ?? false;

        public ValidationException() : this("Validation failed") { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
        public ValidationException(Exception innerException) : this("Validation failed", innerException) { }
    }
}
