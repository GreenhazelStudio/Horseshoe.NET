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
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert " + Alert.AlertType.ToCssClass() + (Alert.Closeable ? " alert-dismissible" : ""));
            writer.AddAttribute("role", "alert");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);                                                // Begin 'bootstrap alert' div

            if (Alert.Closeable)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "close");
                writer.AddAttribute("data-dismiss", "alert");
                writer.AddAttribute("aria-label", "Close");
                writer.RenderBeginTag("button");                                                         // Begin 'close' button
                writer.Write("&times;");
                writer.RenderEndTag();                                                                   // End 'close' button
            }

            if (Alert.Emphasis != null)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);                                         // Begin 'emphasis' strong
                writer.Write(Alert.Emphasis);
                writer.RenderEndTag();                                                                   // End 'emphasis' strong
                writer.Write(" - ");
            }

            // message
            var message = TextUtil.RevealNullOrBlank(Alert.Message);
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
                if ((Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) == AlertMessageDetailsRenderingPolicy.Hidden)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");                       // for 'alert details' div
                }
                else
                { 
                    usePre = (Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;
                    htmlEncoded = (Alert.MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.HtmlEncoded) == AlertMessageDetailsRenderingPolicy.HtmlEncoded;
                    writer.RenderBeginTag(HtmlTextWriterTag.Script);                                     // Begin 'toggle' script
                    writer.Write
                    (
                        @"
                            function ToggleAlertDetails(clickedLink, alertDetailsElementID) {
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
                        "
                    );
                    writer.RenderEndTag();                                                               // End 'toggle' script
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);                                        // Begin 'toggle link' div
                    writer.AddAttribute("href", "javascript:;");
                    writer.AddAttribute("onclick", "ToggleAlertDetails(this, '" + AlertDetailsElementID + "')");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write("show details");
                    writer.RenderEndTag(); // A
                    writer.RenderEndTag(); // Div                                                        // End 'toggle link' div
                    writer.AddAttribute("id", AlertDetailsElementID);                                    // for 'alert details' div
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");                       // ""
                    if (usePre)
                    {
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "Consolas, monospace"); // for 'alert details' div
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, ".8em");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "pre");
                    }
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Div);                                            // Begin 'alert details' div

                var messageDetails = (htmlEncoded ? HttpUtility.HtmlEncode(Alert.MessageDetails) : Alert.MessageDetails);
                if (!usePre)
                {
                    messageDetails = messageDetails.Replace("\n", "<br />\n");
                }
                writer.Write("\n" + messageDetails);

                writer.RenderEndTag();                                                                   // End 'alert details' div
            }

            writer.RenderEndTag();                                                                       // End 'bootstrap alert' div
        }
    }
}
