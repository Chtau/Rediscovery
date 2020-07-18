using SharedBase.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface ILoggerEvent : ILogger
    {
        event EventHandler<LoggerEntry> EntryAdded;
    }
}
