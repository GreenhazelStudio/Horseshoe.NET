using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Horseshoe.NET.Mvc;
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
            return View(Model);
        }

        [HttpPost]
        public ActionResult OriginalRequestBodyTest(int? intValue1, string textValue1)
        {
            var result = 
                "intValue=" + TextUtil.Reveal(intValue1, nullOrBlank: true) + Environment.NewLine + 
                "textValue1=" + TextUtil.Reveal(textValue1, nullOrBlank: true);
            result += Environment.NewLine + "original_request_body=" + Request.GetOriginalRequestBody();
            Model.RequestBodyTestResult = result;
            return RedirectToAction("Index");
        }
    }
}