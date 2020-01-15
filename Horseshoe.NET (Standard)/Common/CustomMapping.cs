using System;

namespace Horseshoe.NET.Common
{
    internal interface ICustomMapping
    {
        string SourcePropertyName { get; }
        string DestPropertyName { get; }
        object GetDestPropertyValue(object sourcePropertyValue);
    }

    internal class CustomMapping<S, D> : ICustomMapping
    {
        public string SourcePropertyName { get; set; }
        public string DestPropertyName { get; set; }
        internal Func<S, D> Convert { get; set; }

        public object GetDestPropertyValue(object sourcePropertyValue)
        {
            var invalidReason = GetInvalidReason();
            if (invalidReason == null)
            {
                return Convert.Invoke((S)sourcePropertyValue);
            }
            throw new UtilityException(invalidReason);
        }

        private string GetInvalidReason()
        {
            if (string.IsNullOrWhiteSpace(SourcePropertyName))
            {
                return "Source property name is not specified";
            }
            if (string.IsNullOrWhiteSpace(DestPropertyName))
            {
                return "Destination property name is not specified";
            }
            if (Convert == null)
            {
                return "A convert function is not specified";
            }
            return null;
        }
    }
}
