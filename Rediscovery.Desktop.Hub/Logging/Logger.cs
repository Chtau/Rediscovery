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
        private readonly ILogger<T> _logger;

        public Logger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger.LogCritical(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogError(Exception exception)
        {
            _logger.LogError(exception, "");
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            _logger.LogTrace(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
