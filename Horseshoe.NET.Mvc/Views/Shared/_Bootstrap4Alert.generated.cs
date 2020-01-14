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
    
    #line 3 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using System.Web;
    
    #line default
    #line hidden
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 4 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET.Mvc;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    using Horseshoe.NET.Text;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Bootstrap4Alert.cshtml")]
    public partial class _Views_Shared__Bootstrap4Alert_cshtml : System.Web.Mvc.WebViewPage<Horseshoe.NET.Mvc.Bootstrap4.Alert>
    {
        public _Views_Shared__Bootstrap4Alert_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n");

            
            #line 10 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
  
    var alertDetailsElementID = "alert-exception-" + Guid.NewGuid();

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 252), Tuple.Create("\"", 405)
, Tuple.Create(Tuple.Create("", 260), Tuple.Create("alert", 260), true)
            
            #line 14 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
, Tuple.Create(Tuple.Create(" ", 265), Tuple.Create<System.Object, System.Int32>(Model.AlertType.ToCssClass() + (Model.Closeable ? " alert-dismissible" : "") + (Model.Fade ? " fade" : "") + (Model.Show ? " show" : "")
            
            #line default
            #line hidden
, 266), false)
);

WriteLiteral(">\r\n");

            
            #line 15 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
    
            
            #line default
            #line hidden
            
            #line 15 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
      
        if (Model.Closeable)
        {

            
            #line default
            #line hidden
WriteLiteral("            <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" class=\"close\"");

WriteLiteral(" data-dismiss=\"alert\"");

WriteLiteral(" aria-label=\"close\"");

WriteLiteral(">&times;</a>\r\n");

            
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
                                       Write(Html.Raw(" - "));

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                                                            
        }

        var message = TextUtil.Reveal(TextUtil.Zap(Model.Message), nullOrBlank: true);
        if (Model.MessageHtmlEncoded)
        {
            message = HttpUtility.HtmlEncode(message);
        }
        message = message.Replace("\n", "<br />\n");

        
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
   Write(Html.Raw(message));

            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                          

        if (Model.MessageDetails != null)
        {
            bool usePre = false;
            bool htmlEncoded = false;
            var messageDetails = Model.MessageDetails;

            if ((Model.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) != AlertMessageDetailsRenderingPolicy.Hidden)
            {
                usePre = (Model.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;
                htmlEncoded = (Model.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.HtmlEncoded) == AlertMessageDetailsRenderingPolicy.HtmlEncoded;

                if (htmlEncoded)
                {
                    messageDetails = HttpUtility.HtmlEncode(messageDetails);
                }
                if (!usePre)
                {
                    messageDetails = messageDetails.Replace("\n", "<br />\n");
                }


            
            #line default
            #line hidden
WriteLiteral("                <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
                    function ShowBootstrapAlertMessageDetails(clickedLink, alertDetailsElementID) {
                        if (window.jQuery) {
                            $('#' + alertDetailsElementID).show();
                            $(clickedLink).hide();
                        }
                        else {
                            var alertDetailsElement = document.getElementById(alertDetailsElementID);
                            alertDetailsElement.style.display = 'block';
                            clickedLink.style.display = 'none';
                        }
                    }
                </script>
");

WriteLiteral("                <div>\r\n                    <a");

WriteLiteral(" href=\"javascript:;\"");

WriteAttribute("onclick", Tuple.Create(" onclick=\"", 2699), Tuple.Create("\"", 2773)
, Tuple.Create(Tuple.Create("", 2709), Tuple.Create("ShowBootstrapAlertMessageDetails(this,", 2709), true)
, Tuple.Create(Tuple.Create(" ", 2747), Tuple.Create("\'", 2748), true)
            
            #line 69 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
            , Tuple.Create(Tuple.Create("", 2749), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 2749), false)
, Tuple.Create(Tuple.Create("", 2771), Tuple.Create("\')", 2771), true)
);

WriteLiteral(">show details</a>\r\n                </div>\r\n");

WriteLiteral("                <div");

WriteAttribute("id", Tuple.Create(" id=\"", 2837), Tuple.Create("\"", 2864)
            
            #line 71 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
, Tuple.Create(Tuple.Create("", 2842), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 2842), false)
);

WriteAttribute("style", Tuple.Create(" style=\"", 2865), Tuple.Create("\"", 2955)
, Tuple.Create(Tuple.Create("", 2873), Tuple.Create("font-family:Consolas,monospace;font-size:.8em;", 2873), true)
            
            #line 71 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
                       , Tuple.Create(Tuple.Create("", 2919), Tuple.Create<System.Object, System.Int32>(usePre ? "white-space:pre;" : ""
            
            #line default
            #line hidden
, 2919), false)
, Tuple.Create(Tuple.Create("", 2954), Tuple.Create(")", 2954), true)
);

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 72 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
               Write(Html.Raw(messageDetails));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n");

            
            #line 74 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                <div");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 78 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
               Write(Html.Raw(messageDetails));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n");

            
            #line 80 "..\..\Views\Shared\_Bootstrap4Alert.cshtml"
            }
        }
    
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</div>\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
