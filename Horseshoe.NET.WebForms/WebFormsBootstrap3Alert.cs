using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET.Bootstrap;
using Horseshoe.NET.Text;

namespace Horseshoe.NET.WebForms
{
    public class WebFormsBootstrap3Alert : WebControl
    {
        public Bootstrap3.Alert Alert { get; }
        string AlertDetailsElementID { get; } = "alert-details-" + Guid.NewGuid();

        public WebFormsBootstrap3Alert(Bootstrap3.Alert alert)
        {
            Alert = alert;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // alert ui
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert " + Alert.AlertType.ToCssClass() + (Alert.Closeable ? " alert-dismissible" : ""));
            writer.AddAttribute("role", "alert");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (Alert.Closeable)
            {
                // begin close button
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "close");
                writer.AddAttribute("data-dismiss", "alert");
                writer.AddAttribute("aria-label", "Close");
                writer.RenderBeginTag("button");

                // close button x
                writer.AddAttribute("aria-hidden", "true");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write("&times;");
                writer.RenderEndTag();

                // finalize close button
                writer.RenderEndTag();
            }

            // message eye catcher
            if (Alert.Emphasis != null)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write(Alert.Emphasis);
                writer.RenderEndTag();
                writer.Write(" - ");
            }

            // message
            var message = TextUtil.Reveal(Alert.Message, nullOrBlank: true);
            if (Alert.MessageEncodeHtml)
            {
                message = HttpUtility.HtmlEncode(message);
            }
            message = message.Replace("\n", "\n<br />");
            writer.Write(message);

            // render message detilas
            if (Alert.MessageDetails != null)
            {
                bool usePre = false;
                bool htmlEncoded = false;
                if ((Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.KeepHidden) != AlertMessageDetailsRenderingPolicy.KeepHidden)
                {
                    usePre = (Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;
                    htmlEncoded = (Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.HtmlEncoded) == AlertMessageDetailsRenderingPolicy.HtmlEncoded;
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);
                    writer.Write
                    (
                        @"
                            function ShowBootstrapAlertMessageDetails(clickedLink, alertDetailsElementID) {
                                if (window.jQuery) {  
                                    $('#' + alertDetailsElementID).show();
                                    $(clickedLink).hide();
                                } 
                                else {
                                    var messageDetailsElement = document.getElementById(alertDetailsElementID);
                                    messageDetailsElement.style.display = 'block';
                                    clickedLink.style.display = 'none';
                                }
                            }
                        "
                    );
                    writer.RenderEndTag(); // Script
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute("href", "javascript:;");
                    writer.AddAttribute("onclick", "ShowBootstrapAlertMessageDetails(this, '" + AlertDetailsElementID + "')");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("show details");
                    writer.RenderEndTag(); // A
                    writer.RenderEndTag(); // Div
                    writer.AddAttribute("id", AlertDetailsElementID);                                // for next Div
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Consolas, monospace"); // for next Div
                    writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, ".8em");                  // for next Div
                }
                if (usePre)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "pre");
                }
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                
                var messageDetails = (htmlEncoded ? HttpUtility.HtmlEncode(Alert.MessageDetails) : Alert.MessageDetails);
                if (!usePre)
                {
                    messageDetails = messageDetails.Replace("\n", "<br />\n");
                }
                writer.Write("\n" + messageDetails);

                writer.RenderEndTag();  // Div
            }

            // finalize alert ui
            writer.RenderEndTag();
        }
    }
}
