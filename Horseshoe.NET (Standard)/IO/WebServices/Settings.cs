using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.IO.WebServices
{
    public static class Settings
    {
        static Credential? _defaultCredentials;

        /// <summary>
        /// Gets or sets how Net will log into web services.  Note: Override by passing directly to a Net function or via config file: key =  "Horseshoe.NET:Net:UserName|Password|Domain"
        /// </summary>
        public static Credential? DefaultCredentials
        {
            get
            {
                return _defaultCredentials
                    ?? Credential.Build(Config.Get("Horseshoe.NET:WebServices:UserName"), Config.Get("Horseshoe.NET:WebServices:Password"), isEncryptedPassword: Config.GetBoolean("Horseshoe.NET:WebServices:IsEncryptedPassword"), domain: Config.Get("Horseshoe.NET:WebServices:Domain"))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("WebServices.Credentials");
            }
            set
            {
                _defaultCredentials = value;
            }
        }
    }
}
