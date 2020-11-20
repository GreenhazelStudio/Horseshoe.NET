using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Horseshoe.NET.Application;

namespace Horseshoe.NET.Text
{
    public static class TextSettings
    {
        private static JsonProvider? _jsonProvider;

        /// <summary>
        /// Gets or sets the default symmetric algorithm used by Cryptography.  Note: Overrides other settings (i.e. app|web.config: key = Horseshoe.NET:Crypto.SymmetricAlgorithm and OrganizationalDefaultSettings: key = Cryptography.SymmetricAlgorithm)
        /// </summary>
        public static JsonProvider JsonProvider
        {
            get
            {
                if (!_jsonProvider.HasValue)
                {
                    _jsonProvider = Config.GetNEnum<JsonProvider>("Horseshoe.NET:Text.JsonProvider", doNotRequireConfiguration: true)
                        ?? OrganizationalDefaultSettings.GetNullable<JsonProvider>("Text.JsonProvider")
                        ?? (IsLoadable("Newtonsoft.Json") ? JsonProvider.NewtonsoftJson as JsonProvider? : null)
                        ?? (IsLoadable("System.Text.Json") ? JsonProvider.SystemTextJson as JsonProvider? : null)
                        ?? JsonProvider.None;
                }
                return _jsonProvider.Value;
            }
            set
            {
                _jsonProvider = value;
            }
        }

        static bool IsLoadable(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(assemblyName));
            if (assembly != null)
            {
                return true;
            }

            try
            {
                assembly = Assembly.Load(assemblyName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
