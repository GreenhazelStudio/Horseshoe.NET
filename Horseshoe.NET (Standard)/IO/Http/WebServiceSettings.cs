using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.IO.Http
{
    public static class WebServiceSettings
    {
        static Credential? _defaultWebServiceCredentials;

        /// <summary>
        /// Gets or sets how Net will log into web services.  Note: Override by passing directly to a Net function or via config file: key =  "Horseshoe.NET:WebService.UserName|Password|Domain"
        /// </summary>
        public static Credential? DefaultWebServiceCredentials
        {
            get
            {
                return _defaultWebServiceCredentials
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("WebService.Credentials");
            }
            set
            {
                _defaultWebServiceCredentials = value;
            }
        }
    }
}
