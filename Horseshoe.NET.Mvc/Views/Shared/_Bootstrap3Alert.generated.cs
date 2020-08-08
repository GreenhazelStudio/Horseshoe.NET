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
    
    #line 3 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    using Horseshoe.NET.Bootstrap;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    using Horseshoe.NET.Bootstrap.Extensions;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
    using Horseshoe.NET.Text;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Bootstrap3Alert.cshtml")]
    public partial class _Views_Shared__Bootstrap3Alert_cshtml : System.Web.Mvc.WebViewPage<Bootstrap3.Alert>
    {
        public _Views_Shared__Bootstrap3Alert_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

WriteLiteral("\r\n");

            
            #line 9 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
  

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteAttribute("class", Tuple.Create(" class=\"", 171), Tuple.Create("\"", 264)
, Tuple.Create(Tuple.Create("", 179), Tuple.Create("alert", 179), true)
            
            #line 10 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create(" ", 184), Tuple.Create<System.Object, System.Int32>(Model.AlertType.ToCssClass() + (Model.Closeable ? " alert-dismissible" : "")
            
            #line default
            #line hidden
, 185), false)
);

WriteLiteral(">\r\n");

            
            #line 11 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
        
            
            #line default
            #line hidden
            
            #line 11 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
          
            if (Model.Closeable)
            {

            
            #line default
            #line hidden
WriteLiteral("                <button");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"close\"");

WriteLiteral(" data-dismiss=\"alert\"");

WriteLiteral(" aria-label=\"close\"");

WriteLiteral(">&times;</button>\r\n");

            
            #line 15 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
            }

            if (Model.Emphasis != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                <strong>");

            
            #line 19 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                   Write(Model.Emphasis);

            
            #line default
            #line hidden
WriteLiteral("</strong>");

            
            #line 19 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                
            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                            Write(" - ");

            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                        
            }

            var message = TextUtil.RevealNullOrBlank(Horseshoe.NET.ObjectClean.Methods.ZapString(Model.Message));
            if (Model.EncodeHtml)
            {
                
            
            #line default
            #line hidden
            
            #line 25 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
           Write(message);

            
            #line default
            #line hidden
            
            #line 25 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                        
            }
            else
            {
                
            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
           Write(Html.Raw(message.Replace("\r\n", "<br />").Replace("\n", "<br />")));

            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                                                    
            }

            if (Model.MessageDetails != null)
            {
                if (Model.IsMessageDetailsHidden)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 37 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                   Write(Html.Raw(Model.MessageDetails));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n");

            
            #line 39 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                }
                else
                {
                    var alertDetailsElementID = "alert-details-" + Guid.NewGuid();

            
            #line default
            #line hidden
WriteLiteral("                    <div>\r\n                        <a");

WriteLiteral(" href=\"javascript:;\"");

WriteAttribute("onclick", Tuple.Create(" onclick=\"", 1412), Tuple.Create("\"", 1483)
, Tuple.Create(Tuple.Create("", 1422), Tuple.Create("Bootstrap3.toggleAlertDetails(this,", 1422), true)
, Tuple.Create(Tuple.Create(" ", 1457), Tuple.Create("\'", 1458), true)
            
            #line 44 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
             , Tuple.Create(Tuple.Create("", 1459), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 1459), false)
, Tuple.Create(Tuple.Create("", 1481), Tuple.Create("\')", 1481), true)
);

WriteLiteral(">show details</a>\r\n                    </div>\r\n");

            
            #line 46 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"

                    var preStyles = Model.IsMessageDetailsPreFormatted ? "font-family:Consolas,monospace;font-size:.8em;white-space:pre;" : "";

            
            #line default
            #line hidden
WriteLiteral("                    <div");

WriteAttribute("id", Tuple.Create(" id=\"", 1702), Tuple.Create("\"", 1729)
            
            #line 48 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create("", 1707), Tuple.Create<System.Object, System.Int32>(alertDetailsElementID
            
            #line default
            #line hidden
, 1707), false)
);

WriteAttribute("style", Tuple.Create(" style=\"", 1730), Tuple.Create("\"", 1761)
, Tuple.Create(Tuple.Create("", 1738), Tuple.Create("display:none;", 1738), true)
            
            #line 48 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
, Tuple.Create(Tuple.Create("", 1751), Tuple.Create<System.Object, System.Int32>(preStyles
            
            #line default
            #line hidden
, 1751), false)
);

WriteLiteral(">\r\n");

            
            #line 49 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 49 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                         if (Model.IsMessageDetailsEncodeHtml)
                        {
                            
            
            #line default
            #line hidden
            
            #line 51 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                       Write(Model.MessageDetails);

            
            #line default
            #line hidden
            
            #line 51 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                 
                        }
                        else
                        {
                            
            
            #line default
            #line hidden
            
            #line 55 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                       Write(Html.Raw(Model.MessageDetails));

            
            #line default
            #line hidden
            
            #line 55 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                                                           
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </div>\r\n");

            
            #line 58 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
                }
            }
        
            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 62 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 63 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
 if (Model.MessageDetails != null && !Model.IsMessageDetailsHidden)
{

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Bootstrap3 = {
            toggleAlertDetails: function (clickedLink, alertDetailsElementID) {
                if (window.jQuery) {
                    var $clickedLink = $(clickedLink);
                    if ($clickedLink.prop(""toggled"")) {
                        $(""#"" + alertDetailsElementID).hide();
                        $clickedLink.text(""show details"");
                        $clickedLink.prop(""toggled"", false);
                    }
                    else {
                        $(""#"" + alertDetailsElementID).show();
                        $clickedLink.text(""hide details"");
                        $clickedLink.prop(""toggled"", true);
                    }
                }
                else {
                    if (clickedLink.toggled) {
                        document.getElementById(alertDetailsElementID).style.display = ""none"";
                        clickedLink.innerText = ""show details"";
                        clickedLink.toggled = false;
                    }
                    else {
                        document.getElementById(alertDetailsElementID).style.display = ""block"";
                        clickedLink.innerText = ""hide details"";
                        clickedLink.toggled = true;
                    }
                }
            }
        }
    </script>
");

            
            #line 96 "..\..\Views\Shared\_Bootstrap3Alert.cshtml"
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
