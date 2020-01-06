using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TestMVC.ViewModels;

namespace TestMVC.Controllers
{
    public class WebController : Controller
    {
        // GET: Web
        public ActionResult Index()
        {
            var vm = new WebIndexViewModel
            {
                HttpRequest = Request,
            };
            return View(vm);
        }
    }
}