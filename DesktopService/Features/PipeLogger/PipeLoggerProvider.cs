using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.PipeLogger
{
    public class PipeLoggerProvider : ILoggerProvider
    {
        private readonly PipeLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, PipeLogger> _loggers = new ConcurrentDictionary<string, PipeLogger>();

        public PipeLoggerProvider(PipeLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new PipeLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
