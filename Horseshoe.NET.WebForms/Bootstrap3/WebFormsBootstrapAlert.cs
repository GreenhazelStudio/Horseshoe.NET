using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using Horseshoe.NET.Text;
using Horseshoe.NET.Web;
using Horseshoe.NET.Web.Bootstrap3;

namespace Horseshoe.NET.WebForms.Bootstrap3
{
    public class WebFormsBootstrapAlert : WebControl
    {
        public string Emphasis { get; }
        public string Message { get; }
        public AlertType AlertType { get; }
        public bool SuppressRenderingNewLines { get; }
        public bool IsCloseable { get; }
        public ExceptionInfo Exception { get; }
        public ExceptionRenderingPolicy ExceptionRenderingPolicy { get; }
        private Guid Guid { get; } = Guid.NewGuid();
        internal string AlertExceptionElementID => "alert-exception-" + Guid.ToString();

        public WebFormsBootstrapAlert(BootstrapAlert bootstrapAlert)
        {
            Emphasis = bootstrapAlert.Emphasis;
            Message = bootstrapAlert.Message;
            AlertType = bootstrapAlert.AlertType;
            SuppressRenderingNewLines = bootstrapAlert.SuppressRenderingNewLines;
            IsCloseable = bootstrapAlert.IsCloseable;
            Exception = bootstrapAlert.Exception;
            ExceptionRenderingPolicy = bootstrapAlert.ExceptionRenderingPolicy;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // alert ui
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert alert-" + AlertType.ToString().ToLower() + (IsCloseable ? " alert-dismissible" : ""));
            writer.AddAttribute("role", "alert");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (IsCloseable)
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
            if (Emphasis != null)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                writer.Write(Emphasis);
                writer.RenderEndTag();
                writer.Write("&nbsp;&nbsp;");
            }

            // message
            var message = TextUtil.Zap(Message);
            if (Exception != null)
            {
                message = message ?? TextUtil.Zap(Exception.Message);
            }
            message = TextUtil.Reveal(message, nullOrBlank: true);
            message = message.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

            if (SuppressRenderingNewLines)
            {
                message = message.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
            }
            else
            {
                message = message.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
            }
            writer.Write(message);

            // render exception
            if (Exception != null)
            {
                var indent = 2;
                switch (ExceptionRenderingPolicy)
                {
                    case ExceptionRenderingPolicy.InAlert:
                        writer.RenderBeginTag(HtmlTextWriterTag.Script);
                        writer.Write
                        (
                            @"
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
                            "
                        );
                        writer.RenderEndTag();

                        var htmlRenderedException = Exception.Render(displayFullClassName: true, displayMessage: !string.Equals(Message, Exception.Message), displayStackTrace: true, indent: indent, recursive: true);
                        htmlRenderedException = htmlRenderedException.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
                        
                        if (indent > 1)
                        {
                            htmlRenderedException = htmlRenderedException.Replace(new string(' ', indent), TextUtil.Repeat("&nbsp;", indent));
                        }

                        if (SuppressRenderingNewLines)
                        {
                            htmlRenderedException = htmlRenderedException.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                        }
                        else
                        {
                            htmlRenderedException = htmlRenderedException.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
                        }

                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.AddAttribute("href", "javascript:;");
                        writer.AddAttribute("onclick", "ShowDerbyUtilitiesNotifyAlertException(this, '" + AlertExceptionElementID + "')");
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write("show details");
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                        writer.AddAttribute("id", AlertExceptionElementID);
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, "'Consolas', monospace");
                        writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, ".8em");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.Write(htmlRenderedException);
                        writer.RenderEndTag();
                        break;
                    case ExceptionRenderingPolicy.InAlertHidden:
                        var renderedException = Exception.Render(displayFullClassName: true, displayMessage: !string.Equals(Message, Exception.Message), displayStackTrace: true, indent: indent, recursive: true);
                        writer.Write
                        (
                            Environment.NewLine + "<!-- Exception -->" +
                            Environment.NewLine + "<div style=\"display:none\">" +
                            Environment.NewLine + renderedException +
                            Environment.NewLine + "</div>" + Environment.NewLine
                        );
                        break;
                }
            }

            // finalize alert ui
            writer.RenderEndTag();
        }
    }
}
