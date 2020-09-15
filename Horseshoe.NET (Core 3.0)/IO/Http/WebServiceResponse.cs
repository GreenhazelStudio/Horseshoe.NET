using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Horseshoe.NET.IO.Http.Enums;

namespace Horseshoe.NET.IO.Http
{
    /// <summary>
    /// A robust, serializable web API response that can help overcome the limitations of exception handling.
    /// By catching and returning exceptions in a normal (HTTP 200) resonse the exception details are guaranteed 
    /// to be preserved.  In some cases ASP.NET actually produces an HTML exception page.  However, in other cases
    /// a server-side error is manifest as a no detail HTTP 500 error.  In either case, sometimes API calls especially 
    /// AJAX calls are easier to handle if the return type is consistent. 
    /// </summary>
    public class WebServiceResponse<E> : WebServiceResponse
    {
        /// <summary>
        /// The data to return to the caller (will be JSONified)
        /// </summary>
        public new E Data 
        {
            get => base.Data != null ? (E)base.Data : default; 
            set => base.Data = value; 
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebServiceResponse() : base()
        {
        }

        /// <summary>
        /// Constructor for a normal response (not used, client code typically only calls contructors w/ params in WSResponse, not WSResponse&lt;E&gt;)
        /// </summary>
        /// <param name="data"></param>
        public WebServiceResponse(E data)
        {
            Data = data;
        }

        /// <summary>
        /// Constructor for an error type response (not used, client code typically only calls contructors w/ params in WSResponse, not WSResponse&lt;E&gt;)
        /// </summary>
        /// <param name="ex"></param>
        public WebServiceResponse(ExceptionInfo ex) : base(ex)
        {
        }
    }

    /// <summary>
    /// A robust, serializable web API response that can help overcome the limitations of exception handling.
    /// By catching and returning exceptions in a normal (HTTP 200) resonse the exception details are guaranteed 
    /// to be preserved.  In some cases ASP.NET actually produces an HTML exception page.  However, in other cases
    /// a server-side error is manifest as a no detail HTTP 500 error.  In either case, sometimes API calls especially 
    /// AJAX calls are easier to handle if the return type is consistent. 
    /// </summary>
    public class WebServiceResponse
    {
        /// <summary>
        /// The data to return to the caller (will be JSONified)
        /// </summary>
        public object Data { get; set; }


        /// <summary>
        /// The exception information to return 
        /// </summary>
        public ExceptionInfo Exception { get; set; }

        /// <summary>
        /// The exception type (for client-side JavaScript)
        /// </summary>
        public string ExceptionType => Exception?.ClassName;

        /// <summary>
        /// The status (i.e. Ok, Error) of this response
        /// </summary>
        public WebServiceResponseStatus Status { get; set; } = WebServiceResponseStatus.Ok;

        /// <summary>
        /// The status (i.e. Ok, Error) of this response (for client-side JavaScript)
        /// </summary>
        public string StatusText => Status.ToString();

        /// <summary>
        /// Use this to easily pass a count of something (e.g. number of rows affected in a data operation)
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// Use this to easily pass a note for client code / developers
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public WebServiceResponse()
        {
        }

        /// <summary>
        /// Constructor for a normal response
        /// </summary>
        /// <param name="data"></param>
        public WebServiceResponse(object data)
        {
            Data = data;
        }

        /// <summary>
        /// Constructor for an error type response
        /// </summary>
        /// <param name="ex"></param>
        public WebServiceResponse(ExceptionInfo ex)
        {
            Exception = ex;
            Status = WebServiceResponseStatus.Error;
        }
    }
}