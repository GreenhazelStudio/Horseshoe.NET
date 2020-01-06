using Horseshoe.NET.Web.Bootstrap4;

namespace Horseshoe.NET.WebForms.Bootstrap4
{
    public static class Extensions
    {
        public static WebFormsBootstrapAlert ToControl(this BootstrapAlert bootstrapAlert)
        {
            return new WebFormsBootstrapAlert(bootstrapAlert);
        }
    }
}
