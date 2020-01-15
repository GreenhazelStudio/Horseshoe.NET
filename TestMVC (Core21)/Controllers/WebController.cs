using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET;
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
            return View(Model);
        }

        [EnableOriginalRequestBody]
        public ActionResult OriginalRequestBodyTest(int? intValue1, string textValue1)
        {
            var result =
                "intValue=" + TextUtil.Reveal(intValue1, nullOrBlank: true) + Environment.NewLine +
                "textValue1=" + TextUtil.Reveal(textValue1, nullOrBlank: true);
            result += Environment.NewLine + "original_request_body=" + Request.GetOriginalRequestBody().Trunc(80, truncPolicy: TruncatePolicy.Ellipsis);
            var model = Model;
            model.RequestBodyTestResult = result;
            Model = model;
            return RedirectToAction("Index");
        }
    }
}