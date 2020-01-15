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
            public bool MessageEncodeHtml { get; set; }
            public string Emphasis { get; set; }
            public bool Closeable { get; set; }
            public string MessageDetails { get; set; }
            public AlertMessageDetailsRenderingPolicy MessageDetailsRendering { get; set; }
        }

        public static Alert CreateAlert
        (
            AlertType alertType,
            string message,
            bool messageEncodeHtml = false,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            string messageDetails = null,
            AlertMessageDetailsRenderingPolicy messageDetailsRendering = default
        )
        {
            return new Alert
            {
                AlertType = alertType,
                Message = message,
                MessageEncodeHtml = messageEncodeHtml,
                Emphasis = emphasis ?? (autoEmphasis ? alertType.ToString() : null),
                Closeable = closeable,
                MessageDetails = messageDetails,
                MessageDetailsRendering = messageDetailsRendering
            };
        }

        public static Alert CreateInfoAlert
        (
            string message,
            bool encodeMessageHtml = false,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
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
                messageEncodeHtml: encodeMessageHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateSuccessAlert
        (
            string message,
            bool encodeMessageHtml = false,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
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
                messageEncodeHtml: encodeMessageHtml,
                messageDetails: messageDetails,
                messageDetailsRendering: messageDetailsRendering
            );
        }

        public static Alert CreateWarningAlert
        (
            string message,
            bool encodeMessageHtml = false,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
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
                messageEncodeHtml: encodeMessageHtml,
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
            bool encodeMessageHtml = false,
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
                messageEncodeHtml: encodeMessageHtml,
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
            bool encodeMessageHtml = false,
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
                messageEncodeHtml: encodeMessageHtml,
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
            bool displayFullClassName = false,
            bool displayMessageInErrorDetails = true,
            bool displayStackTrace = false,
            int indent = 2,
            bool recursive = false,
            ExceptionRenderingPolicy? errorRendering = null
        )
        {
            var resultantErrorRendering = errorRendering ?? Settings.DefaultExceptionRendering;
            return CreateAlert
            (
                AlertType.Error,
                exception?.Message ?? "[null]",
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                messageEncodeHtml: true,
                messageDetails: resultantErrorRendering != default
                    ? exception?.Render(displayFullClassName: displayFullClassName, displayMessage: displayMessageInErrorDetails, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive)
                    : null,
                messageDetailsRendering: resultantErrorRendering.ToAlertMessageDetailsRendering()
            );
        }

    }
}
