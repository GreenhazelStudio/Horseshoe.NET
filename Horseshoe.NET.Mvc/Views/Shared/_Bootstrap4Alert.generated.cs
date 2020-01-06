﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 3 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET.Text;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET.Web;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET.Web.Bootstrap4;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Bootstrap4Alert.cshtml")]
    public partial class _Views_Shared__Bootstrap4Alert_cshtml : System.Web.Mvc.WebViewPage<Horseshoe.NET.Web.Bootstrap4.BootstrapAlert>
    {
        public _Views_Shared__Bootstrap4Alert_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\n\n");

WriteLiteral("\n");

            
            #line 10 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
  
    var alertExceptionElementID = "alert-exception-" + Guid.NewGuid();

            
            #line default
            #line hidden
WriteLiteral("\n\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 268), Tuple.Create("\"", 442)
, Tuple.Create(Tuple.Create("", 276), Tuple.Create("alert", 276), true)
            
            #line 14 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
, Tuple.Create(Tuple.Create(" ", 281), Tuple.Create<System.Object, System.Int32>("alert-" + Model.AlertType.ToString().ToLower() + (Model.IsCloseable ? " alert-dismissible" : "") + (Model.Fade ? " fade" : "") + (Model.Show ? " show" : "")
            
            #line default
            #line hidden
, 282), false)
);

WriteLiteral(">\n");

            
            #line 15 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    
            
            #line default
            #line hidden
            
            #line 15 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
      
        if (Model.IsCloseable)
        {

            
            #line default
            #line hidden
WriteLiteral("            <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" class=\"close\"");

WriteLiteral(" data-dismiss=\"alert\"");

WriteLiteral(" aria-label=\"close\"");

WriteLiteral(">&times;</a>\n");

            
            #line 19 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
        }

        if (Model.Emphasis != null)
        {

            
            #line default
            #line hidden
WriteLiteral("            <strong>");

            
            #line 23 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
               Write(Model.Emphasis);

            
            #line default
            #line hidden
WriteLiteral("</strong>");

            
            #line 23 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                                            
            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                                       Write(Html.Raw("&nbsp;&nbsp;"));

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                                                                     
        }

        var message = TextUtil.Zap(Model.Message);
        if (Model.Exception != null)
        {
            message = message ?? TextUtil.Zap(Model.Exception.Message);
        }
        message = TextUtil.Reveal(message, nullOrBlank: true);
        message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

        if (Model.SuppressRenderingNewLines)
        {
            message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
        }
        else
        {
            message = message.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
        }
    
            
            #line default
            #line hidden
WriteLiteral("\n\n");

WriteLiteral("    ");

            
            #line 44 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
Write(Html.Raw(message));

            
            #line default
            #line hidden
WriteLiteral("\n\n");

            
            #line 46 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
     if (Model.Exception != null)
    {
        var indent = 2;
        switch (Model.ExceptionRenderingPolicy)
        {
            case ExceptionRenderingPolicy.InAlert:


            
            #line default
            #line hidden
WriteLiteral("                <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
                    function ShowDerbyUtilitiesNotifyAlertException(clickedLink, alertExceptionElementID) {
                        if (window.jQuery) {
                            $('#' + alertExceptionElementID).show();
                            $(clickedLink).hide();
                        }
                        else {
                            var alertExceptionElement = document.getElementById(alertExceptionElementID);
                            alertExceptionElement.style.display = 'block';
                            clickedLink.style.display = 'none';
                        }
                    }
                </script>
");

            
            #line 66 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"

                var htmlRenderedException = Model.Exception.Render(displayFullClassName: true, displayMessage: !string.Equals(Model.Message, Model.Exception.Message), displayStackTrace: true, indent: indent, recursive: true);
                htmlRenderedException = htmlRenderedException.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

                if (indent > 1)
                {
                    htmlRenderedException = htmlRenderedException.Replace(new string(' ', indent), TextUtil.Repeat("&nbsp;", indent));
                }

                if (Model.SuppressRenderingNewLines)
                {
                    htmlRenderedException = htmlRenderedException.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                }
                else
                {
                    htmlRenderedException = htmlRenderedException.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
                }


            
            #line default
            #line hidden
WriteLiteral("                <div>\n                    <a");

WriteLiteral(" href=\"javascript:;\"");

WriteAttribute("onclick", Tuple.Create(" onclick=\"", 3288), Tuple.Create("\"", 3370)
, Tuple.Create(Tuple.Create("", 3298), Tuple.Create("ShowDerbyUtilitiesNotifyAlertException(this,", 3298), true)
, Tuple.Create(Tuple.Create(" ", 3342), Tuple.Create("\'", 3343), true)
            
            #line 85 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                  , Tuple.Create(Tuple.Create("", 3344), Tuple.Create<System.Object, System.Int32>(alertExceptionElementID
            
            #line default
            #line hidden
, 3344), false)
, Tuple.Create(Tuple.Create("", 3368), Tuple.Create("\')", 3368), true)
);

WriteLiteral(">show details</a>\n                </div>\n");

WriteLiteral("                <div");

WriteAttribute("id", Tuple.Create(" id=\"", 3432), Tuple.Create("\"", 3461)
            
            #line 87 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
, Tuple.Create(Tuple.Create("", 3437), Tuple.Create<System.Object, System.Int32>(alertExceptionElementID
            
            #line default
            #line hidden
, 3437), false)
);

WriteLiteral(" style=\"display: none;font-family: Consolas,monospace; font-size: .8em\"");

WriteLiteral(">\n");

WriteLiteral("                    ");

            
            #line 88 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
               Write(Html.Raw(htmlRenderedException));

            
            #line default
            #line hidden
WriteLiteral("\n                </div>\n");

            
            #line 90 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                break;
            case ExceptionRenderingPolicy.InAlertHidden:
                var renderedException = Model.Exception.Render(displayFullClassName: true, displayMessage: !string.Equals(Model.Message, Model.Exception.Message), displayStackTrace: true, indent: indent, recursive: true);

            
            #line default
            #line hidden
WriteLiteral("                <!-- Exception -->\n");

WriteLiteral("                <div");

WriteLiteral(" style=\"display:none\"");

WriteLiteral(">\n");

WriteLiteral("                    ");

            
            #line 95 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
               Write(Html.Raw(renderedException));

            
            #line default
            #line hidden
WriteLiteral("\n                </div>\n");

            
            #line 97 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                break;
        }
    }

            
            #line default
            #line hidden
WriteLiteral("\n</div>\n");

        }
    }
}
#pragma warning restore 1591
