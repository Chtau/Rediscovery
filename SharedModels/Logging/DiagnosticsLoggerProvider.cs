using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SharedBase.Logging
{
    public sealed class DiagnosticsLoggerProvider : ILogger
    {
        #region Singleton
        private static readonly DiagnosticsLoggerProvider instance = new DiagnosticsLoggerProvider();
        static DiagnosticsLoggerProvider()
        {
        }
        private DiagnosticsLoggerProvider()
        {
        }
        public static DiagnosticsLoggerProvider Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        public event EventHandler<LoggerEntry> EntryAdded;

        public void LogCritical(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, nameof(LogCritical));
            Debug.Indent();
        }

        public void LogDebug(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, nameof(LogDebug));
            Debug.Indent();
        }

        public void LogError(Exception exception)
        {
            Debug.WriteLine(exception.ToString(), nameof(LogError));
            Debug.Indent();
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(exception.ToString(), nameof(LogError));
            Debug.WriteLine(message, nameof(LogError));
            Debug.Indent();
        }

        public void LogInformation(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, nameof(LogInformation));
            Debug.Indent();
        }

        public void LogTrace(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, nameof(LogTrace));
            Debug.Indent();
        }

        public void LogWarning(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, nameof(LogWarning));
            Debug.Indent();
        }
    }
}
