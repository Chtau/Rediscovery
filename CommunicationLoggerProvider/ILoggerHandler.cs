using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Logger.Provider
{
    public interface ILoggerHandler
    {
        bool Pause { get; set; }
        int MaxEntires { get; set; }
        event EventHandler EntriesChanged;

        void NewEntry(Rediscovery.Shared.Base.Logging.LoggerEntry loggerEntry);
        List<Rediscovery.Shared.Base.Logging.LoggerEntry> Get();
        void ClearEntries();
        Rediscovery.Shared.Base.Logging.LogCommandConfigResult ExecuteCommand(Rediscovery.Shared.Base.Logging.LogCommandConfig logCommandConfig);
    }
}
