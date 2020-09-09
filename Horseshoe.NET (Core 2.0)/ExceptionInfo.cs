using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    /// <summary>
    /// A basic custom class suitable for communicating information about .NET exceptions over JSON
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// The fully qualified name of the type of the original exception
        /// </summary>
        public string FullClassName { get; set; }

        /// <summary>
        /// The name of the type of the original exception
        /// </summary>
        public string ClassName
        {
            get
            {
                if (FullClassName != null)
                {
                    int pos = FullClassName.LastIndexOf(".");
                    if (pos > -1)
                    {
                        return FullClassName.Substring(pos + 1);
                    }
                }
                return FullClassName;
            }
        }

        /// <summary>
        /// The message copied over from the original exception
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The stack trace copied over from the original exception
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Inner exceptions (in the form of ExceptionInfo) copied over from the original exception 
        /// </summary>
        public ExceptionInfo InnerException { get; set; }

        /// <summary>
        /// Creates an instance of ExceptionInfo from an exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ExceptionInfo From(Exception exception)
        {
            if (exception == null) return null;

            var exceptionInfo = new ExceptionInfo
            {
                FullClassName = exception is ReconstitutedException reconstitutedException
                    ? reconstitutedException.ClassName
                    : exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                InnerException = From(exception.InnerException)
            };

            return exceptionInfo;
        }

        public static implicit operator ExceptionInfo(Exception ex) => From(ex);

        public static implicit operator Exception(ExceptionInfo exInfo) => ReconstitutedException.From(exInfo);
    }
}
