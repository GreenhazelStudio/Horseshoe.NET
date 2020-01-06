using System;

namespace Horseshoe.NET.Web.Bootstrap3
{
    /* References http://getbootstrap.com/docs/3.3/components/#alerts */
    public class BootstrapAlert
    {
        public string Emphasis { get; set; }
        public string Message { get; set; }
        public AlertType AlertType { get; set; }
        public bool SuppressRenderingNewLines { get; set; }
        public bool IsCloseable { get; set; }
        public ExceptionInfo Exception { get; set; }
        public ExceptionRenderingPolicy ExceptionRenderingPolicy { get; set; }

        public BootstrapAlert() { }

        public BootstrapAlert(string message, AlertType alertType = default, string emphasis = null, bool isCloseable = false)
        {
            Message = message;
            AlertType = alertType;
            Emphasis = emphasis;
            IsCloseable = isCloseable;
        }

        public BootstrapAlert(Exception exception, string message = null, AlertType alertType = AlertType.Danger, string emphasis = null, bool isCloseable = false, ExceptionRenderingPolicy? exceptionRenderingPolicy = null) : this (ExceptionInfo.From(exception), message: message, alertType: alertType, emphasis: emphasis, isCloseable: isCloseable, exceptionRenderingPolicy: exceptionRenderingPolicy) { }

        public BootstrapAlert(ExceptionInfo exception, string message = null, AlertType alertType = AlertType.Danger, string emphasis = null, bool isCloseable = false, ExceptionRenderingPolicy? exceptionRenderingPolicy = null)
        {
            Message = message ?? exception.Message;
            AlertType = alertType;
            Emphasis = emphasis;
            IsCloseable = isCloseable;
            Exception = exception;
            ExceptionRenderingPolicy = exceptionRenderingPolicy ?? Settings.DefaultExceptionRenderingPolicy;
        }
    }
}
