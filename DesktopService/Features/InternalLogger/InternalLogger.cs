using Microsoft.Extensions.Logging;
using System;

namespace Rediscovery.Client.App.Service.Features.InternalLogger
{
    public class InternalLogger : ILogger
    {
        private readonly string _name;
        private readonly InternalLoggerConfiguration _config;

        public InternalLogger(string name, InternalLoggerConfiguration config)
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
                var color = Console.ForegroundColor;
                Console.ForegroundColor = _config.Color;
                Console.WriteLine($"{logLevel} - {eventId.Id} - {_name}{Environment.NewLine}{formatter(state, exception)}");
                Console.ForegroundColor = color;
            }
        }
    }
}