using System;

namespace Horseshoe.NET.WebForms
{
    [Flags]
    public enum AlertMessageDetailsRenderingPolicy
    {
        Default = 0,
        HtmlEncoded = 1,
        PreFormatted = 2,
        Hidden = 4
    }
}
