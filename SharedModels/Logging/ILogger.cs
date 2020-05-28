using System;
using System.Collections.Generic;
using System.Text;

namespace SharedBase.Logging
{
    public interface ILogger
    {
        event EventHandler<LoggerEntry> EntryAdded;
        void LogCritical(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogError(Exception exception);
        void LogError(Exception exception, string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogTrace(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }
}
