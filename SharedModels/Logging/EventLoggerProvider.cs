using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Logging
{
    public class EventLoggerProvider : ILogger
    {
        #region Singleton
        private static readonly EventLoggerProvider instance = new EventLoggerProvider();
        static EventLoggerProvider()
        {
        }
        private EventLoggerProvider()
        {
        }
        public static EventLoggerProvider Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        public event EventHandler<LoggerEntry> LogNewEntry;

        private void OnLogEntry(string message, LoggerEntry.LoggerType loggerType, string module = null)
        {
            LogNewEntry?.Invoke(this, new LoggerEntry
            {
                Id = Guid.NewGuid().ToString(),
                LogLevel = loggerType,
                Message = message,
                Module = module,
                Time = DateTime.Now,
            });
        }

        public void LogCritical(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
           OnLogEntry(message, LoggerEntry.LoggerType.Critical, nameof(LogCritical));
        }

        public void LogDebug(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            OnLogEntry(message, LoggerEntry.LoggerType.Debug, nameof(LogDebug));
        }

        public void LogError(Exception exception)
        {
            OnLogEntry(exception.ToString(), LoggerEntry.LoggerType.Error, nameof(LogError));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            OnLogEntry(exception.ToString() + Environment.NewLine + message, LoggerEntry.LoggerType.Error, nameof(LogError));
        }

        public void LogInformation(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            OnLogEntry(message, LoggerEntry.LoggerType.Information, nameof(LogInformation));
        }

        public void LogTrace(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            OnLogEntry(message, LoggerEntry.LoggerType.Trace, nameof(LogTrace));
        }

        public void LogWarning(string message, params object[] args)
        {
            if (args?.Length > 0)
                message = string.Format(message, args);
            OnLogEntry(message, LoggerEntry.LoggerType.Warning, nameof(LogWarning));
        }
    }
}
