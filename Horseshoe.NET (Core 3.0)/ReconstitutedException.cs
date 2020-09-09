using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    /// <summary>
    /// A specialized exception class for rehydrating and rethrowing instances of <see cref="ExceptionInfo"/> (e.g. from a web service call, see <see cref="GenericResponse"/>)
    /// </summary>
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "Only used in deserialized scenarios")]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Only used in deserialized scenarios")]
    public sealed class ReconstitutedException : Exception
    {
        /// <summary>
        /// The fully qualified name of the type of the original exception
        /// </summary>
        public string FullClassName => (string)Data["FullClassName"];

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
        /// The stack trace copied over from the original exception
        /// </summary>
        public override string StackTrace => (string)Data["StackTrace"];

        private ReconstitutedException(ExceptionInfo info) : base(info.Message)
        {
            Data.Add("FullClassName", info.FullClassName);
            Data.Add("StackTrace", info.StackTrace);
        }

        private ReconstitutedException(ExceptionInfo info, ExceptionInfo innerException) : base
        (
            info.Message,
            innerException.InnerException != null
                ? new ReconstitutedException(innerException, innerException.InnerException)
                : new ReconstitutedException(innerException)
        )
        {
            Data.Add("FullClassName", info.FullClassName);
            Data.Add("StackTrace", info.StackTrace);
        }

        public static ReconstitutedException From(ExceptionInfo info)
        {
            return info.InnerException != null
                ? new ReconstitutedException(info, info.InnerException)
                : new ReconstitutedException(info);
        }
    }
}
