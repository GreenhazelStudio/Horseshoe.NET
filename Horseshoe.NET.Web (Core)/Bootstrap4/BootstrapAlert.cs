using System;

namespace Horseshoe.NET.Web.Bootstrap4
{
    /* References http://getbootstrap.com/docs/3.3/components/#alerts */
    public class BootstrapAlert
    {
        public string Emphasis { get; set; }
        public string Message { get; set; }
        public AlertType AlertType { get; set; }
        public bool SuppressRenderingNewLines { get; set; }
        public bool IsCloseable { get; set; }
        public bool Fade { get; set; }
        public bool Show { get; set; }
        public ExceptionInfo Exception { get; set; }
        public ExceptionRenderingPolicy ExceptionRenderingPolicy { get; set; }

        public BootstrapAlert() { }

        public BootstrapAlert(string message, AlertType alertType = default, string emphasis = null, bool isCloseable = false, bool fade = true, bool show = true)
        {
            Message = message;
            AlertType = alertType;
            Emphasis = emphasis;
            IsCloseable = isCloseable;
            Fade = fade;
            Show = show;
        }

        public BootstrapAlert(Exception exception, string message = null, AlertType alertType = AlertType.Danger, string emphasis = null, bool isCloseable = false, bool fade = true, bool show = true, ExceptionRenderingPolicy? exceptionRenderingPolicy = null) : this (ExceptionInfo.From(exception), message: message, alertType: alertType, emphasis: emphasis, isCloseable: isCloseable, fade: fade, show: show, exceptionRenderingPolicy: exceptionRenderingPolicy) { }

        public BootstrapAlert(ExceptionInfo exception, string message = null, AlertType alertType = AlertType.Danger, string emphasis = null, bool isCloseable = false, bool fade = true, bool show = true, ExceptionRenderingPolicy? exceptionRenderingPolicy = null)
        {
            Message = message ?? exception.Message;
            AlertType = alertType;
            Emphasis = emphasis;
            IsCloseable = isCloseable;
            Fade = fade;
            Show = show;
            Exception = exception;
            ExceptionRenderingPolicy = exceptionRenderingPolicy ?? Settings.DefaultExceptionRenderingPolicy;
        }
    }
}
