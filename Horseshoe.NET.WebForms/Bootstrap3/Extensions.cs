using Horseshoe.NET.Web.Bootstrap3;

namespace Horseshoe.NET.WebForms.Bootstrap3
{
    public static class Extensions
    {
        public static WebFormsBootstrapAlert ToControl(this BootstrapAlert bootstrapAlert)
        {
            return new WebFormsBootstrapAlert(bootstrapAlert);
        }
    }
}
