using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Horseshoe.NET.IO.Http.Enums;

using Newtonsoft.Json;

namespace Horseshoe.NET.IO.Http
{
    /// <summary>
    /// A custom Web API action result that can help overcome the limitations of exception result handling by treating caught
    /// exceptions as part of a standard HTTP 200 responses thereby preserving the exceptions' details. 
    /// This version of GenericResponse (with type parameter) is typically used for decoding API call responses after they are received by the caller.
    /// </summary>
    public class WebServiceResponse<E> where E : class
    {
        /// <summary>
        /// The data to return to the caller (will be JSONified)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public E Data { get; set; }

        /// <summary>
        /// The exception information to return 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionInfo Exception { get; set; }

        /// <summary>
        /// The exception type (for client-side JavaScript)
        /// </summary>
        [JsonProperty]
        public string ExceptionType => Exception?.ClassName;

        /// <summary>
        /// The status (i.e. Ok, Error) of this response
        /// </summary>
        [JsonProperty]
        public WebServiceResponseStatus Status { get; set; } = WebServiceResponseStatus.Ok;

        /// <summary>
        /// The status (i.e. Ok, Error) of this response (for client-side JavaScript)
        /// </summary>
        [JsonProperty]
        public string StatusText => Status.ToString();

        /// <summary>
        /// Use this to easily pass a count of something (e.g. number of rows affected in a data operation)
        /// </summary>
        [JsonProperty]
        public int Count { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebServiceResponse()
        {
        }

        /// <summary>
        /// Constructor for a normal response (not used, client code typically only calls contructors w/ params in WSResponse, not WSResponse&lt;E&gt;)
        /// </summary>
        /// <param name="data"></param>
        public WebServiceResponse(E data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Constructor for an error type response (not used, client code typically only calls contructors w/ params in WSResponse, not WSResponse&lt;E&gt;)
        /// </summary>
        /// <param name="ex"></param>
        public WebServiceResponse(ExceptionInfo ex)
        {
            this.Exception = ExceptionInfo.From(ex);
            Status = WebServiceResponseStatus.Error;
        }

        /// <summary>
        /// Constructor for an error type response that includes data (not used, client code typically only calls contructors w/ params in GenericResponse, not GenericResponse&lt;E&gt;)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public WebServiceResponse(E data, ExceptionInfo ex) : this(data)
        {
            this.Exception = ex;
            Status = WebServiceResponseStatus.Error;
        }
    }

    /// <summary>
    /// A custom Web API action result that can help overcome the limitations of exception result handling by treating caught
    /// exceptions as part of a standard HTTP 200 responses thereby preserving the exceptions' details. 
    /// This version of GenericResponse (no type parameter) is typically used for encoding the API call responses to be returned to the caller.
    /// </summary>
    public class WebServiceResponse : WebServiceResponse<object>
    {
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
        public WebServiceResponse(object data) : base(data)
        {
        }

        /// <summary>
        /// Constructor for an error type response
        /// </summary>
        /// <param name="ex"></param>
        public WebServiceResponse(ExceptionInfo ex) : base(ex)
        {
        }

        /// <summary>
        /// Constructor for an error type response that includes data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ex"></param>
        public WebServiceResponse(object data, ExceptionInfo ex) : base(data, ex) 
        {
        }
    }
}