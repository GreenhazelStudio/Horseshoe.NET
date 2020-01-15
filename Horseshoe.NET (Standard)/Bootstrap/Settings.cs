using System;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Bootstrap
{
    public static class Settings
    {
        private static ExceptionRenderingPolicy? _defaultExceptionRendering;

        public static ExceptionRenderingPolicy DefaultExceptionRendering  // example "InAlert"
        {
            get
            {
                return _defaultExceptionRendering
                    ?? GetExceptionRenderingPolicy(Config.GetNEnum<ExceptionRenderingPolicy>("Horseshoe.NET:Bootstrap:ExceptionRendering"))
                    ?? GetExceptionRenderingPolicy(OrganizationalDefaultSettings.GetNullable<ExceptionRenderingPolicy>("Bootstrap.ExceptionRendering"))
                    ?? ExceptionRenderingPolicy.Preclude;
            }
            set
            {
                _defaultExceptionRendering = value;
            }
        }

        static ExceptionRenderingPolicy? GetExceptionRenderingPolicy(ExceptionRenderingPolicy? exceptionRendering)
        {
            if (exceptionRendering == ExceptionRenderingPolicy.Dynamic)
            {
                switch (ClientApp.AppMode)
                {
                    case AppMode.Production:
                    case AppMode.IA:
                    case AppMode.QA:
                    case AppMode.UAT:
                    case AppMode.Training:
                        return ExceptionRenderingPolicy.Preclude;
                    case AppMode.Development:
                        return ExceptionRenderingPolicy.ToggleToView;
                    case AppMode.Test:
                        return ExceptionRenderingPolicy.KeepHidden;
                }
            }
            return exceptionRendering;
        }
    }
}
