using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Logging
{
    public class CrossDeviceLogger : ILogger
    {
        public void LogCritical(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogDebug(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogInformation(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogTrace(string message, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void LogWarning(string message, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
