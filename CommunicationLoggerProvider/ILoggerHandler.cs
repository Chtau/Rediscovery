using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerProvider
{
    public interface ILoggerHandler
    {
        event EventHandler<List<SharedBase.Logging.LoggerEntry>> EntriesChanged;

        void NewEntry(SharedBase.Logging.LoggerEntry loggerEntry);
        List<SharedBase.Logging.LoggerEntry> Get();
    }
}
