using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Horseshoe.NET.Application;
using Horseshoe.NET.Mvc.Extensions;

namespace TestMVC.ViewModels
{
    public class UtilityIndexViewModel
    {
        public AppType? AppType { get; set; }
        public StringBuilder AppTypeMessageTracker { get; set; }
        public string AppTypeMessageTrackerHTML => AppTypeMessageTracker?.ToString().Replace(Environment.NewLine, "<br />");
        public HttpRequestBase HttpRequest { get; set; }
        public string AbsoluteApplicationPath => HttpRequest.GetAbsoluteApplicationPath();
        public string AbsoluteApplicationPath_API => HttpRequest.GetAbsoluteApplicationPath(virtualSubpath: "/api");
    }
}