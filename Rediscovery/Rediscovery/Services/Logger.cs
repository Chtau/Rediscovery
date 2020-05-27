using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.Logger))]
namespace Rediscovery.Services
{
    public class Logger : SharedBase.Logging.ILogger
    {
        public void LogCritical(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogCritical(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogDebug(message, args);
        }

        public void LogError(Exception exception)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogError(exception);
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogError(exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogInformation(message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogTrace(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogWarning(message, args);
        }
    }
}
