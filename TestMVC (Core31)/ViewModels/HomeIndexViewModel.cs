using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Mvc;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TestMVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public HttpRequest HttpRequest { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; set; }
        public string AbsoluteApplicationPath => HttpRequest.GetAbsoluteApplicationPath();
        public string AbsoluteApplicationPath_API => HttpRequest.GetAbsoluteApplicationPath(virtualSubpath: "/api");
    }
}
