using Rediscovery.Shared.Logging.Commands;
using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Provider.Logger
{
    public interface ILoggerHandler
    {
        bool Pause { get; set; }
        int MaxEntires { get; set; }
        event EventHandler EntriesChanged;

        void NewEntry(LoggerEntry loggerEntry);
        List<LoggerEntry> Get();
        void ClearEntries();
        LogCommandConfigResult ExecuteCommand(LogCommandConfig logCommandConfig);
    }
}
