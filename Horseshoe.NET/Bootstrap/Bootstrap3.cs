using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Bootstrap
{
    public static class Bootstrap3
    {
        public enum AlertType
        {
            Info,
            Success,
            Warning,
            Danger,
            Error
        }

        /* References http://getbootstrap.com/docs/3.3/components/#alerts */
        public class Alert
        {
            public AlertType AlertType { get; set; }
            public string Message { get; set; }
            public string Emphasis { get; set; }
            public bool Closeable { get; set; }
            public bool EncodeHtml { get; set; }
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
            bool displayShortName = false,
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
                messageDetails: resultantErrorRendering != default
                    ? exception?.Render(displayShortName: displayShortName, displayMessage: displayMessageInErrorDetails, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive)
                    : null,
                messageDetailsRendering: resultantErrorRendering.ToAlertMessageDetailsRendering()
            );
        }

    }
}
