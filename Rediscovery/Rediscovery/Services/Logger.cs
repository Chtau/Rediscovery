using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.Logger))]
namespace Rediscovery.Services
{
    public class Logger : SharedBase.Logging.ILogger
    {
        public event EventHandler<LoggerEntry> EntryAdded;

        public void LogCritical(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogCritical(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), string.Format(message, args), LoggerEntry.LoggerType.Critical));
        }

        public void LogDebug(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogDebug(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), string.Format(message, args), LoggerEntry.LoggerType.Debug));
        }

        public void LogError(Exception exception)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogError(exception);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString(), LoggerEntry.LoggerType.Error));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogError(exception, message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString() + Environment.NewLine + string.Format(message, args), LoggerEntry.LoggerType.Error));
        }

        public void LogInformation(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogInformation(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), string.Format(message, args), LoggerEntry.LoggerType.Information));
        }

        public void LogTrace(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogTrace(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), string.Format(message, args), LoggerEntry.LoggerType.Trace));
        }

        public void LogWarning(string message, params object[] args)
        {
            SharedBase.Logging.DiagnosticsLoggerProvider.Instance.LogWarning(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery), string.Format(message, args), LoggerEntry.LoggerType.Warning));
        }
    }
}
