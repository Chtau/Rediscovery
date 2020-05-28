using Microsoft.Extensions.Logging;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Logging
{
    public class Logger<T> : SharedBase.Logging.ILogger
    {
        public event EventHandler<LoggerEntry> EntryAdded;

        private readonly ILogger<T> _logger;

        public Logger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), string.Format(message, args), LoggerEntry.LoggerType.Critical));
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), string.Format(message, args), LoggerEntry.LoggerType.Debug));
        }

        public void LogError(Exception exception)
        {
            _logger.LogError(exception, "");
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), exception.ToString(), LoggerEntry.LoggerType.Error));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), exception.ToString() + Environment.NewLine + string.Format(message, args), LoggerEntry.LoggerType.Error));
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), string.Format(message, args), LoggerEntry.LoggerType.Information));
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), string.Format(message, args), LoggerEntry.LoggerType.Trace));
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
            EntryAdded?.Invoke(this, LoggerEntry.CreateEntry(nameof(Rediscovery.Desktop.Hub), string.Format(message, args), LoggerEntry.LoggerType.Warning));
        }
    }
}
