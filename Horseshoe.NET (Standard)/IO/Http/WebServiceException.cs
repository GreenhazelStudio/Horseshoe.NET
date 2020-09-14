using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.Http
{
    public class WebServiceException : Exception
    {
        public WebServiceException() : base() 
        {
        }

        public WebServiceException(string message) : base(message) 
        {
        }

        public WebServiceException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }
}
