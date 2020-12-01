using Rediscovery.Communication.Provider.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Logger
{
    public class RemoteLogger : ILogger
    {
        private readonly string _name;
        private readonly RemoteLoggerConfiguration _config;

        public RemoteLogger(string name, RemoteLoggerConfiguration config)
        {
            _name = name;
            _config = config;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                _config?.GetLoggerHandlerInstance()?.NewEntry(new Shared.Logging.Models.LoggerEntry
                {
                    Id = eventId.Id.ToString(),
                    LogLevel = GetLoggerType(logLevel),
                    Message = formatter(state, exception),
                    Time = DateTime.Now,
                    Module = _config.LoggingModuleName,
                    Sid = !string.IsNullOrWhiteSpace(_config.LoggingModuleName) ? _config.LoggingModuleName : "DesktopService"
                });
            }
        }

        private Shared.Logging.LoggerType GetLoggerType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Shared.Logging.LoggerType.Trace;
                case LogLevel.Debug:
                    return Shared.Logging.LoggerType.Debug;
                case LogLevel.Information:
                case LogLevel.None:
                    return Shared.Logging.LoggerType.Information;
                case LogLevel.Warning:
                    return Shared.Logging.LoggerType.Warning;
                case LogLevel.Error:
                    return Shared.Logging.LoggerType.Error;
                case LogLevel.Critical:
                    return Shared.Logging.LoggerType.Critical;
            }
            return Shared.Logging.LoggerType.Information;
        }
    }
}
