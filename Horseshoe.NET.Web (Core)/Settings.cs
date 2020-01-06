using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Web
{
    public static class Settings
    {
        private static ExceptionRenderingPolicy? _defaultExceptionRenderingPolicy;

        public static ExceptionRenderingPolicy DefaultExceptionRenderingPolicy  // example "InAlert"
        {
            get
            {
                return _defaultExceptionRenderingPolicy
                    ?? GetExceptionRenderingPolicy(Config.GetNEnum<ExceptionRenderingPolicy>("Horseshoe.NET:Web:ExceptionRenderingPolicy"))
                    ?? GetExceptionRenderingPolicy(OrganizationalDefaultSettings.GetNullable<ExceptionRenderingPolicy>("Web.ExceptionRenderingPolicy"))
                    ?? ExceptionRenderingPolicy.None;
            }
            set
            {
                _defaultExceptionRenderingPolicy = value;
            }
        }

        static ExceptionRenderingPolicy? GetExceptionRenderingPolicy(ExceptionRenderingPolicy? policy)
        {
            if (policy == ExceptionRenderingPolicy.Dynamic)
            {
                switch (ClientApp.AppMode)
                {
                    case AppMode.Production:
                    case AppMode.IA:
                    case AppMode.QA:
                    case AppMode.UAT:
                    case AppMode.Training:
                        return ExceptionRenderingPolicy.None;
                    case AppMode.Development:
                        return ExceptionRenderingPolicy.InAlert;
                    case AppMode.Test:
                        return ExceptionRenderingPolicy.InAlertHidden;
                }
            }
            return policy;
        }
    }
}
