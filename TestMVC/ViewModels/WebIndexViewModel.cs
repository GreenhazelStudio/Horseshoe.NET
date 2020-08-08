using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.Mvc.Extensions;

namespace TestMVC.ViewModels
{
    public class WebIndexViewModel
    {
        public HttpRequestBase HttpRequest { get; set; }
        public string AbsoluteApplicationPath => HttpRequest.GetAbsoluteApplicationPath();
        public string AbsoluteApplicationPath_API => HttpRequest.GetAbsoluteApplicationPath(virtualSubpath: "/api");
        public string RequestBodyTestResult { get; set; }
        public Bootstrap3.Alert BootstrapAlert { get; set; }
    }
}