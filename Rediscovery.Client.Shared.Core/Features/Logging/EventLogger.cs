using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Shared.Logging;
using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Logging
{
    public class EventLogger : ILogger
    {
        public void LogCritical(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerType.Critical));
        }

        public void LogDebug(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerType.Debug));
        }

        public void LogError(Exception exception)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString(), LoggerType.Error));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString() + Environment.NewLine + OnFormat(message, args), LoggerType.Error));
        }

        public void LogInformation(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerType.Information));
        }

        public void LogTrace(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerType.Trace));
        }

        public void LogWarning(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerType.Warning));
        }

        private string OnFormat(string message, params object[] args)
        {
            if (args?.Length > 0)
            {
                var stringArgs = new List<string>();
                foreach (var item in args)
                {
                    stringArgs.Add(item.ToString());
                }
                return string.Format(message, stringArgs);
            }
            return message;
        }

        private void OnLogEntry(LoggerEntry loggerEntry)
        {
            if (loggerEntry != null)
            {
                Resolver.Get<ILoggingData>().AddEntry(loggerEntry);
            }
        }
    }
}
