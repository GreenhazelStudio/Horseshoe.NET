using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.Mvc.Extensions;
using Horseshoe.NET.Text;

using TestMVC.ViewModels;

namespace TestMVC.Controllers
{
    public class WebController : Controller
    {
        WebIndexViewModel Model
        {
            get
            {
                WebIndexViewModel model = (WebIndexViewModel)Session["WebIndexViewModel"];
                if (model == null)
                {
                    Session["WebIndexViewModel"] = model = new WebIndexViewModel();
                }
                return model;
            }
        }

        // GET: Web
        public ActionResult Index()
        {
            Model.HttpRequest = Request;
            DisplayAlerts();
            return View(Model);
        }

        public ActionResult DisplayInfoAlert()
        {
            Model.BootstrapAlert = Bootstrap3.CreateInfoAlert("Plain message.");
            return RedirectToAction("Index");
        }

        public ActionResult DisplayInfoHtmlAlert()
        {
            Model.BootstrapAlert = Bootstrap3.CreateInfoAlert("<b>H</b><u>T</u><i>M</i><span style=\"color:red;\">L</span> message.", messageDetails: "<strong>strong details</strong>");
            return RedirectToAction("Index");
        }

        public ActionResult DisplayInfoHtmlEncodedAlert()
        {
            Model.BootstrapAlert = Bootstrap3.CreateInfoAlert("<b>H</b><u>T</u><i>M</i><span style=\"color:red;\">L</span> message.", encodeHtml: true, messageDetails: "<strong>strong details</strong>", messageDetailsRendering: AlertMessageDetailsRenderingPolicy.EncodeHtml);
            return RedirectToAction("Index");
        }

        public ActionResult DisplayCloseableErrorAlert()
        {
            try 
            {
                ExceptionMethod1();
            }
            catch (Exception ex)
            {
                Model.BootstrapAlert = Bootstrap3.CreateErrorAlert(ex, closeable: true, exceptionRendering: ExceptionRenderingPolicy.Visible);
            }
            return RedirectToAction("Index");
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
            throw new Exception("Exception in Method #3 <html>");
        }

        [HttpPost]
        public ActionResult OriginalRequestBodyTest(int? intValue1, string textValue1)
        {
            var result = 
                "intValue=" + TextUtil.RevealNullOrBlank(intValue1) + Environment.NewLine + 
                "textValue1=" + TextUtil.RevealNullOrBlank(textValue1);
            result += Environment.NewLine + "original_request_body=" + Request.GetOriginalRequestBody();
            Model.RequestBodyTestResult = result;
            return RedirectToAction("Index");
        }

        void DisplayAlerts()
        {
            ViewBag.BootstrapAlert = Model.BootstrapAlert;
            Model.BootstrapAlert = null;
        }
    }
}