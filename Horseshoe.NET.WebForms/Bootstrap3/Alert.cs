﻿using System;

namespace Horseshoe.NET.WebForms.Bootstrap3
{
    /* References http://getbootstrap.com/docs/3.3/components/#alerts */
    public class Alert
    {
        public AlertType AlertType { get; set; }
        public string Message { get; set; }
        public string Emphasis { get; set; }
        public string MessageDetails { get; set; }
        public bool Closeable { get; set; }
        public bool MessageHtmlEncoded { get; set; }
        public AlertMessageDetailsRenderingPolicy MessageDetailsRendering { get; set; }

        public static Alert Create
        (
            AlertType alertType,
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeMessageHtml = false
        )
        {
            return new Alert
            {
                AlertType = alertType,
                Message = message,
                Emphasis = emphasis ?? (autoEmphasis ? alertType.ToString() : null),
                Closeable = closeable,
                MessageHtmlEncoded = encodeMessageHtml
            };
        }

        public static Alert CreateInfoAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeMessageHtml = false
        )
        {
            return Create
            (
                AlertType.Info,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeMessageHtml: encodeMessageHtml
            );
        }

        public static Alert CreateSuccessAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeMessageHtml = false
        )
        {
            return Create
            (
                AlertType.Success,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeMessageHtml: encodeMessageHtml
            );
        }

        public static Alert CreateWarningAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            bool closeable = false,
            bool encodeMessageHtml = false
        )
        {
            return Create
            (
                AlertType.Warning,
                message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                closeable: closeable,
                encodeMessageHtml: encodeMessageHtml
            );
        }

        public static Alert CreateErrorAlert
        (
            string message,
            string emphasis = null,
            bool autoEmphasis = true,
            string errorDetails = null,
            bool closeable = false,
            bool encodeMessageHtml = false,
            ExceptionRenderingPolicy? errorRendering = null,
            AlertMessageDetailsRenderingPolicy? errorDetailsRendering = null
        )
        {
            var resultantExceptionRendering = errorRendering ?? Settings.DefaultErrorRendering;
            return new Alert
            {
                AlertType = AlertType.Error,
                Message = message,
                Emphasis = emphasis ?? (autoEmphasis ? AlertType.Error.ToString() : null),
                MessageDetails = errorDetails,
                Closeable = closeable,
                MessageHtmlEncoded = encodeMessageHtml,
                MessageDetailsRendering = errorDetailsRendering ?? resultantExceptionRendering.ToAlertMessageDetailsRendering()
            };
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
            ExceptionRenderingPolicy? errorRendering = null,
            AlertMessageDetailsRenderingPolicy? errorDetailsRendering = null
        )
        { 
            var resultantExceptionRendering = errorRendering ?? Settings.DefaultErrorRendering;
            return CreateErrorAlert
            (
                exception.Message,
                emphasis: emphasis,
                autoEmphasis: autoEmphasis,
                errorDetails: exception.Render(displayFullClassName: displayFullClassName, displayMessage: displayMessageInErrorDetails, displayStackTrace: displayStackTrace, indent: indent, recursive: recursive),
                closeable: closeable,
                errorRendering: resultantExceptionRendering,
                errorDetailsRendering: errorDetailsRendering ?? resultantExceptionRendering.ToAlertMessageDetailsRendering()
            );
        }
    }
}
