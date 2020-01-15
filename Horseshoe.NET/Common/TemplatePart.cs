using System.Linq;

using Horseshoe.NET.Collections;
using Horseshoe.NET.Objects;

namespace Horseshoe.NET.Common
{
    internal class TemplatePart                                     // e.g. Template -> "?key1={{Prop.Value1:M/d/yyyy}}&key2={{Prop.Value2}}"
    {
        internal string RawTemplatePart { get; set; }               // e.g. "{{Prop.Value1:M/d/yyyy}}"

        internal string PropertyName { get; set; }                  // e.g. "Prop.Value1"

        internal string Format { get; set; }                        // e.g. "M/d/yyyy"

        internal string GetReplacementTemplatePart(object value)    // e.g. "5/16/2012"
        {
            string format = Format == null ? "" : ":" + Format;
            return string.Format("{0" + format + "}", value.GetNestedPropertyValue(PropertyName).Value);
        }

        internal static TemplatePart Parse(string rawTemplatePart)
        {
            var templatePart = new TemplatePart
            {
                RawTemplatePart = rawTemplatePart,
            };
            rawTemplatePart = rawTemplatePart.Remove(rawTemplatePart.Length - 2).Remove(0, 2);  // strip off the '{{' and the '}}'
            var parts = rawTemplatePart.Split(':');
            CollectionUtil.Zap(parts);
            if (parts.Any(part => part == null))
            {
                throw new UtilityException("Malformed template");
            }
            templatePart.PropertyName = parts[0];
            if (parts.Length > 1)
            {
                templatePart.Format = parts[1];
            }
            return templatePart;
        }
    }
}
