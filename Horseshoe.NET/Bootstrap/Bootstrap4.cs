using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Bootstrap
{
    public static class Bootstrap4
    {
        public enum AlertType
        {
            Info,
            Success,
            Warning,
            Danger,
            Error,
            Primary,
            Secondary,
            Light,
            Dark
        }

        /* References https://getbootstrap.com/docs/4.1/components/alerts/ */
        public class Alert
        {
            public AlertType AlertType { get; set; }
            public string Message { get; set; }
            public string Emphasis { get; set; }
            public bool Closeable { get; set; }
            public bool EncodeHtml { get; set; }
            public bool Fade { get; set; }
            public bool Show { get; set; }
            public string MessageDetails { get; set; }
            public AlertMessageDetailsRenderingPolicy MessageDetailsRendering { get; set; }
            public bool IsMessageDetailsEncodeHtml => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.EncodeHtml) == AlertMessageDetailsRenderingPolicy.EncodeHtml;
            public bool IsMessageDetailsPreFormatted => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.PreFormatted) == AlertMessageDetailsRenderingPolicy.PreFormatted;
            public bool IsMessageDetailsHidden => (MessageDetailsRendering & AlertMessageDetailsRenderingPolicy.Hidden) == AlertMessageDetailsRenderingPolicy.Hidden;
        }

        public static Alert CreateAlert
        (
            AlertType alertType,
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return new Alert
            {
                AlertType = alertType,
                Message = message,
                Emphasis = emphasis ?? (autoEmphasis ? alertType.ToString() : null),
                Closeable = closeable,
                EncodeHtml = encodeHtml,
                Fade = fade,
                Show = show,
                MessageDetails = messageDetails,
                MessageDetailsRendering = messageDetailsRendering
            };
        }

        public static Alert CreateInfoAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Info,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateSuccessAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Success,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateWarningAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Warning,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                fade: fade,
                show: show,
                encodeHtml: encodeHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateDangerAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Danger,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreatePrimaryAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Primary,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateSecondaryAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Secondary,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateDarkAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Dark,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateLightAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Light,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateErrorAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = false,
            bool fade = true,
            bool show = true,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return CreateAlert
            (
                AlertType.Error,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateErrorAlert
        (
            ExceptionInfo exception,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeHtml = true,
            bool fade = true,
            bool show = true,
            bool displayFullClassName = false,
            bool displayMessageInErrorDetails = true,
            bool displayStackTrace = true,
            int indent = 2,
            bool recursive = false,
            ExceptionRenderingPolicy? exceptionRendering = null
        )
        {
            var resultantErrorRendering = exceptionRendering ?? Settings.DefaultExceptionRendering;
            return CreateAlert
            (
                AlertType.Error,
                exception?.Message ?? "[null]",
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeHtml: encodeHtml,
                fade: fade,
                show: show,
                messageDetails: resultantErrorRendering != default
                    ? exception?.Render(displayFullClassName: displayFullClassName, displayMessage: displayMessageInErrorDetails, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive)
                    : null,
                messageDetailsRendering: resultantErrorRendering.ToAlertMessageDetailsRendering()
            );
        }

    }
}
