﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.IO.Sms
{
    public static class SmsSettings
    {
        static string _defaultFrom;

        /// <summary>
        /// Gets or sets the default sender address used by SMS.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Email.SMS:From and OrganizationalDefaultSettings: key = Email.SMS.From)
        /// </summary>
        public static string DefaultFrom
        {
            get
            {
                return _defaultFrom
                    ?? Config.Get("Horseshoe.NET:Sms.From")
                    ?? OrganizationalDefaultSettings.GetString("Sms.From");
            }
            set
            {
                _defaultFrom = value;
            }
        }
    }
}
