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
    
    #line 3 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
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
    
    #line 4 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    using Horseshoe.NET.Bootstrap;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    using Horseshoe.NET.Text;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Bootstrap3Alert.cshtml")]
    public partial class _Views_Shared__Bootstrap3Alert_cshtml : System.Web.Mvc.WebViewPage<Horseshoe.NET.Bootstrap.Bootstrap3.Alert>
    {
        public _Views_Shared__Bootstrap3Alert_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n");

            
            #line 9 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
  
    var alertDetailsElementID = "alert-exception-" + Guid.NewGuid();

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteAttribute("class", Tuple.Create(" class=\"", 242), Tuple.Create("\"", 335)
, Tuple.Create(Tuple.Create("", 250), Tuple.Create("alert", 250), true)
            
            #line 13 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create(" ", 255), Tuple.Create<System.Object, System.Int32>(Model.AlertType.ToCssClass() + (Model.Closeable ? " alert-dismissible" : "")
            
            #line default
            #line hidden
, 256), false)
);

WriteLiteral(">\r\n");

            
            #line 14 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
      
        if (Model.Closeable)
        {

            
            #line default
            #line hidden
WriteLiteral("            <button");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"close\"");

WriteLiteral(" data-dismiss=\"alert\"");

WriteLiteral(" aria-label=\"close\"");

WriteLiteral(">&times;</button>\r\n");

            
            #line 18 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
        }

        if (Model.Emphasis != null)
        {

            
            #line default
            #line hidden
WriteLiteral("            <strong>");

            
            #line 22 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
               Write(Model.Emphasis);

            
            #line default
            #line hidden
WriteLiteral("</strong>");

            
            #line 22 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                            
            
            #line default
            #line hidden
            
            #line 22 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                       Write(Html.Raw(" - "));

            
            #line default
            #line hidden
            
            #line 22 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                            
        }

        var message = TextUtil.RevealNullOrBlank(TextUtil.Zap(Model.Message));
        if (Model.MessageEncodeHtml)
        {
            message = HttpUtility.HtmlEncode(message);
        }
        message = message.Replace("\n", "<br />\n");

        
            
            #line default
            #line hidden
            
            #line 32 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
   Write(Html.Raw(message));

            
            #line default
            #line hidden
            
            #line 32 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                          

        if (Model.MessageDetails != null)
        {
            bool usePre = false;
            bool htmlEncoded = false;
            var messageDetails = Model.MessageDetails;

            if ((Model.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) == AlertMessageDetailsRenderingPolicy.Hidden)
            {

            
            #line default
            #line hidden
WriteLiteral("                <div");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 43 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
               Write(Html.Raw(messageDetails));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n");

            
            #line 45 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
            }
            else
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

WriteLiteral(">\r\n                    function ToggleAlertDetails(clickedLink, alertDetailsEleme" +
"ntID) {\r\n                        if (window.jQuery) {\r\n                         " +
"   var $clickedLink = $(clickedLink);\r\n                            if ($clickedL" +
"ink.prop(\"toggled\")) {\r\n                                $(\"#\" + alertDetailsElem" +
"entID).hide();\r\n                                $clickedLink.text(\"show details\"" +
");\r\n                                $clickedLink.prop(\"toggled\", false);\r\n      " +
"                      }\r\n                            else {\r\n                   " +
"             $(\"#\" + alertDetailsElementID).show();\r\n                           " +
"     $clickedLink.text(\"hide details\");\r\n                                $clicke" +
"dLink.prop(\"toggled\", true);\r\n                            }\r\n                   " +
"     }\r\n                        else {\r\n                            if (clickedL" +
"ink.toggled) {\r\n                                document.getElementById(alertDet" +
"ailsElementID).style.display = \"none\";\r\n                                clickedL" +
"ink.innerText = \"show details\";\r\n                                clickedLink.tog" +
"gled = false;\r\n                            }\r\n                            else {" +
"\r\n                                document.getElementById(alertDetailsElementID)" +
".style.display = \"block\";\r\n                                clickedLink.innerText" +
" = \"hide details\";\r\n                                clickedLink.toggled = true;\r" +
"\n                            }\r\n                        }\r\n                    }" +
"\r\n                </script>\r\n");

WriteLiteral("                <div>\r\n                    <a");

WriteLiteral(" href=\"javascript:;\"");

WriteAttribute("onclick", Tuple.Create(" onclick=\"", 3701), Tuple.Create("\"", 3761)
, Tuple.Create(Tuple.Create("", 3711), Tuple.Create("ToggleAlertDetails(this,", 3711), true)
, Tuple.Create(Tuple.Create(" ", 3735), Tuple.Create("\'", 3736), true)
            
            #line 90 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create("", 3737), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 3737), false)
, Tuple.Create(Tuple.Create("", 3759), Tuple.Create("\')", 3759), true)
);

WriteLiteral(">show details</a>\r\n                </div>\r\n");

WriteLiteral("                <div");

WriteAttribute("id", Tuple.Create(" id=\"", 3825), Tuple.Create("\"", 3852)
            
            #line 92 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create("", 3830), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 3830), false)
);

WriteAttribute("style", Tuple.Create(" style=\"", 3853), Tuple.Create("\"", 3956)
, Tuple.Create(Tuple.Create("", 3861), Tuple.Create("display:none;", 3861), true)
            
            #line 92 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create("", 3874), Tuple.Create<System.Object, System.Int32>(usePre ? "font-family:Consolas,monospace;font-size:.8em;white-space:pre;" : ""
            
            #line default
            #line hidden
, 3874), false)
, Tuple.Create(Tuple.Create("", 3955), Tuple.Create(")", 3955), true)
);

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 93 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
               Write(Html.Raw(messageDetails));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n");

            
            #line 95 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
            }
        }
    
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
