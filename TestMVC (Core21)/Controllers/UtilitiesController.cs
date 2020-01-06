using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Application;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TestMVC.ViewModels;

namespace TestMVC.Controllers
{
    public class UtilitiesController : Controller
    {
        public IActionResult Index()
        {
            var appTypeMessageTracker = new StringBuilder();
            var vm = new UtilityIndexViewModel
            {
                AppType = ClientApp.DetectAppType(appTypeMessageTracker),
                AppTypeMessageTracker = appTypeMessageTracker
            };
            return View(vm);
        }
    }
}