using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Logger
{
    public class RemoteLoggerProvider : ILoggerProvider
    {
        public static string CachedLastMessage = null;

        private readonly RemoteLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, RemoteLogger> _loggers = new ConcurrentDictionary<string, RemoteLogger>();

        public RemoteLoggerProvider(RemoteLoggerConfiguration config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new RemoteLogger(name, _config));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
