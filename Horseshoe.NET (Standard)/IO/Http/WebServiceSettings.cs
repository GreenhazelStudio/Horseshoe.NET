using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;

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
                    ?? Credential.Build(Config.Get("Horseshoe.NET:WebService.UserName"), Config.Get("Horseshoe.NET:WebService.Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:WebService.IsEncryptedPassword"), domain: Config.Get("Horseshoe.NET:WebService.Domain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("WebService.Credentials");
            }
            set
            {
                _defaultWebServiceCredentials = value;
            }
        }
    }
}
