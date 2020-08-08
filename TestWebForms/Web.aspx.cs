using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.WebForms.Extensions;

namespace TestWebForms
{
    public partial class Web : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void infoAlertTest_Click(object sender, EventArgs e)
        {
            var alert = Bootstrap3.CreateInfoAlert("Polly want a cracker!");
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void infoAlertHtmlTest_Click(object sender, EventArgs e)
        {
            var alert = Bootstrap3.CreateInfoAlert("Polly want a cracker! <em><u>S</u>qu<b>aw</b>k!</em>");
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void infoAlertHtmlEncodeTest_Click(object sender, EventArgs e)
        {
            var alert = Bootstrap3.CreateInfoAlert("Polly want a cracker! <em><u>S</u>qu<b>aw</b>k!</em>", encodeHtml: true);
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void closeableWarningAlert_Click(object sender, EventArgs e)
        {
            var alert = Bootstrap3.CreateWarningAlert("What's shakin' bacon?", closeable: true);
            var alertControl = alert.ToControl();
            BootstrapAlertArea.Controls.Add(alertControl);
        }

        protected void errorAlertHtmlTest_Click(object sender, EventArgs e)
        {
            try
            {
                ExceptionMethod1();
            }
            catch (Exception ex)
            {
                var alert = Bootstrap3.CreateErrorAlert(ex, exceptionRendering: ExceptionRenderingPolicy.Visible);
                var alertControl = alert.ToControl();
                BootstrapAlertArea.Controls.Add(alertControl);
            }
        }

        protected void errorAlertHtmlEncodeTest_Click(object sender, EventArgs e)
        {
            try
            {
                ExceptionMethod1();
            }
            catch (Exception ex)
            {
                var alert = Bootstrap3.CreateErrorAlert(ex, exceptionRendering: ExceptionRenderingPolicy.Visible);
                var alertControl = alert.ToControl();
                BootstrapAlertArea.Controls.Add(alertControl);
            }
        }

        void ExceptionMethod1()
        {
            ExceptionMethod2();
        }

        void ExceptionMethod2()
        {
            ExceptionMethod3();
        }

        void ExceptionMethod3()
        {
            throw new Exception("Exception in Method #3 <i>i</i>");
        }
    }
}