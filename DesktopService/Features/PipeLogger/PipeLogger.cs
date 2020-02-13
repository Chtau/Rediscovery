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
            return logLevel == _config.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_config.EventId == 0 || _config.EventId == eventId.Id)
            {
                Console.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {_name}{Environment.NewLine}{formatter(state, exception)}");
            }
        }
    }
}
