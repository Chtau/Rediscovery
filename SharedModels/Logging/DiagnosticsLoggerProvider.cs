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

        public void LogCritical(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, "Critical");
            Debug.Indent();
        }

        public void LogDebug(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, "Debug");
            Debug.Indent();
        }

        public void LogError(Exception exception)
        {
            Debug.WriteLine(exception.ToString(), "Error");
            Debug.Indent();
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(exception.ToString(), "Error");
            Debug.WriteLine(message, "Error");
            Debug.Indent();
        }

        public void LogInformation(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, "Information");
            Debug.Indent();
        }

        public void LogTrace(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, "Trace");
            Debug.Indent();
        }

        public void LogWarning(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            Debug.WriteLine(message, "Warning");
            Debug.Indent();
        }
        #endregion


    }
}
