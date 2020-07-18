using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerProvider
{
    public interface ILoggerHandler
    {
        bool Pause { get; set; }
        int MaxEntires { get; set; }
        event EventHandler EntriesChanged;

        void NewEntry(SharedBase.Logging.LoggerEntry loggerEntry);
        List<SharedBase.Logging.LoggerEntry> Get();
        void ClearEntries();
        bool ExecuteCommand(SharedBase.Logging.LogCommandConfig logCommandConfig);
    }
}
