using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Horseshoe.NET.Mvc;

namespace TestMVC.Controllers
{
    public class HomeController : Controller
    {
        string OriginalRequestBody
        {
            get => (string)Session["OriginalRequestBody"];
            set
            {
                if (value != null)
                {
                    Session["OriginalRequestBody"] = value;
                }
                else
                {
                    Session.Remove("OriginalRequestBody");
                }
            }
        }

        public ActionResult Index()
        {
            ViewBag.OriginalRequestBody = OriginalRequestBody;
            OriginalRequestBody = null;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public ActionResult OriginalRequestBodyTest(int? intValue1, string textValue1)
        {
            var result =
            //    "intValue=" + TextUtil.RevealNullOrBlank(intValue1) + Environment.NewLine +
            //    "textValue1=" + TextUtil.RevealNullOrBlank(textValue1);
            //result += Environment.NewLine + 
            //    "original_request_body=" + 
            OriginalRequestBody = Request.GetOriginalRequestBody();
            //Model.RequestBodyTestResult = result;
            return RedirectToAction("Index");
        }
    }
}