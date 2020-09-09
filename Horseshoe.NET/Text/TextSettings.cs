using Horseshoe.NET.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
                    _jsonProvider = Config.GetNEnum<JsonProvider>("Horseshoe.NET:Text.JsonProvider")
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
            //Console.WriteLine("Searching... " + assemblyName);
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name.Equals(assemblyName));
            if (assembly != null)
            {
                //Console.WriteLine(" in domain -> " + assembly.FullName);
                return true;
            }

            try
            {
                assembly = Assembly.Load(assemblyName);
                //Console.WriteLine(" loaded -> " + assembly.FullName);
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine(" failed");
                return false;
            }
        }
    }
}
