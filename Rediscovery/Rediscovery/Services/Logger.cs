using Rediscovery.Features.Connection;
using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.Logger))]
namespace Rediscovery.Services
{
    public class Logger : ILoggerEvent
    {
        private IConnectService connectService => DependencyService.Get<IConnectService>();

        [Obsolete]
        public event EventHandler<LoggerEntry> EntryAdded;

        public void LogCritical(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerEntry.LoggerType.Critical));
        }

        public void LogDebug(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerEntry.LoggerType.Debug));
        }

        public void LogError(Exception exception)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString(), LoggerEntry.LoggerType.Error));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), exception.ToString() + Environment.NewLine + OnFormat(message, args), LoggerEntry.LoggerType.Error));
        }

        public void LogInformation(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerEntry.LoggerType.Information));
        }

        public void LogTrace(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerEntry.LoggerType.Trace));
        }

        public void LogWarning(string message, params object[] args)
        {
            OnLogEntry(LoggerEntry.CreateEntry(nameof(Rediscovery), OnFormat(message, args), LoggerEntry.LoggerType.Warning));
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
            connectService.InvokeLogEntry(loggerEntry);
        }
    }
}
