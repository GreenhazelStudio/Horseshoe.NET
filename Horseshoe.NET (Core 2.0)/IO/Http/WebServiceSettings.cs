﻿using System;
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
                    ?? Credential.Build(Config.Get("Horseshoe.NET:WebService.UserName", suppressErrorIfConfigurationServiceNotLoaded: true), Config.Get("Horseshoe.NET:WebService.Password", suppressErrorIfConfigurationServiceNotLoaded: true), isEncryptedPassword: Config.GetBool("Horseshoe.NET:WebService.IsEncryptedPassword", suppressErrorIfConfigurationServiceNotLoaded: true), domain: Config.Get("Horseshoe.NET:WebService.Domain", suppressErrorIfConfigurationServiceNotLoaded: true))
                    ?? OrganizationalDefaultSettings.GetNullable<Credential>("WebService.Credentials");
            }
            set
            {
                _defaultWebServiceCredentials = value;
            }
        }
    }
}
