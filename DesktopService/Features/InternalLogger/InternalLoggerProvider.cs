using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.InternalLogger
{
    public class InternalLoggerProvider : ILoggerProvider
    {
        private readonly InternalLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, InternalLogger> _loggers = new ConcurrentDictionary<string, InternalLogger>();

        public InternalLoggerProvider(InternalLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new InternalLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
