using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.Mvc;
using Horseshoe.NET.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TestMVC.ViewModels;

namespace TestMVC.Controllers
{
    public class WebController : Controller
    {
        ISession Session => HttpContext.Session;

        WebIndexViewModel Model
        {
            get
            {
                WebIndexViewModel model = Session.Get<WebIndexViewModel>("WebIndexViewModel");
                return model ?? new WebIndexViewModel();
            }
            set
            {
                Session.Set("WebIndexViewModel", value);
            }
        }

        public IActionResult Index()
        {
            var model = Model;
            SetAlert(null);
            return View(model);
        }

        public IActionResult DisplayInfoAlert()
        {
            SetAlert(Bootstrap3.CreateInfoAlert("Plain message."));
            return RedirectToAction("Index");
        }

        public IActionResult DisplayInfoHtmlAlert()
        {
            SetAlert(Bootstrap3.CreateInfoAlert("<b>H</b><u>T</u><i>M</i><span style=\"color:red;\">L</span> message.", messageDetails: "<strong>strong details</strong>"));
            return RedirectToAction("Index");
        }

        public IActionResult DisplayInfoHtmlEncodedAlert()
        {
            SetAlert(Bootstrap3.CreateInfoAlert("<b>H</b><u>T</u><i>M</i><span style=\"color:red;\">L</span> message.", encodeHtml: true, messageDetails: "<strong>strong details</strong>", messageDetailsRendering: AlertMessageDetailsRenderingPolicy.EncodeHtml));
            return RedirectToAction("Index");
        }

        public IActionResult DisplayCloseableErrorAlert()
        {
            try
            {
                ExceptionMethod1();
            }
            catch (Exception ex)
            {
                SetAlert(Bootstrap3.CreateErrorAlert(ex, closeable: true, exceptionRendering: ExceptionRenderingPolicy.Visible));
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

        void SetAlert(Bootstrap3.Alert bootstrapAlert)
        {
            var model = Model;
            model.BootstrapAlert = bootstrapAlert;
            Model = model;
        }

        [EnableOriginalRequestBody]
        public ActionResult OriginalRequestBodyTest(int? intValue1, string textValue1)
        {
            var result =
                "intValue=" + TextUtil.RevealNullOrBlank(intValue1) + Environment.NewLine +
                "textValue1=" + TextUtil.RevealNullOrBlank(textValue1);
            result += Environment.NewLine + "original_request_body=" + Request.GetOriginalRequestBody().Crop(80, truncateMarker: TruncateMarker.Ellipsis);
            var model = Model;
            model.RequestBodyTestResult = result;
            Model = model;
            return RedirectToAction("Index");
        }
    }
}