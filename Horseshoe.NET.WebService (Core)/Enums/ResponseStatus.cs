using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Horseshoe.NET.WebServices.Enums
{
    /// <summary>
    /// Basic Web API response status
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// Indicates an exception has occurred
        /// </summary>
        Error, 

        /// <summary>
        /// Indicates a normal response
        /// </summary>
        Ok
    }
}