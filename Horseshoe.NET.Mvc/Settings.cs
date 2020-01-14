using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Mvc
{
    public static class Settings
    {
        private static ExceptionRenderingPolicy? _defaultErrorRendering;

        public static ExceptionRenderingPolicy DefaultErrorRendering  // example "InAlert"
        {
            get
            {
                return _defaultErrorRendering
                    ?? GetErrorRenderingPolicy(Config.GetNEnum<ExceptionRenderingPolicy>("Horseshoe.NET:Mvc:ErrorRendering"))
                    ?? GetErrorRenderingPolicy(OrganizationalDefaultSettings.GetNullable<ExceptionRenderingPolicy>("Mvc.ErrorRendering"))
                    ?? ExceptionRenderingPolicy.None;
            }
            set
            {
                _defaultErrorRendering = value;
            }
        }

        static ExceptionRenderingPolicy? GetErrorRenderingPolicy(ExceptionRenderingPolicy? errorRendering)
        {
            if (errorRendering == ExceptionRenderingPolicy.Dynamic)
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
            return errorRendering;
        }
    }
}
