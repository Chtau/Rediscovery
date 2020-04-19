using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.PipeLogger
{
    public class PipeLogger : ILogger
    {
        private readonly string _name;
        private readonly PipeLoggerConfiguration _config;

        public PipeLogger(string name, PipeLoggerConfiguration config)
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
                _config.PipeLiveLogger.Log(new SharedCoreModels.LoggerEntryModel
                {
                    Id = eventId.Id.ToString(),
                    LogLevel = GetLoggerType(logLevel),
                    Text = formatter(state, exception),
                    Time = DateTime.Now
                });
            }
        }

        private SharedCoreModels.LoggerEntryModel.LoggerType GetLoggerType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Trace;
                case LogLevel.Debug:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Debug;
                case LogLevel.Information:
                case LogLevel.None:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Information;
                case LogLevel.Warning:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Warning;
                case LogLevel.Error:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Error;
                case LogLevel.Critical:
                    return SharedCoreModels.LoggerEntryModel.LoggerType.Critical;
            }
            return SharedCoreModels.LoggerEntryModel.LoggerType.Information;
        }
    }
}
