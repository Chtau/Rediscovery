using Rediscovery.Shared.Logging.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Features.Logging
{
    public interface ILoggingData
    {
        event EventHandler AddedNewEntries;
        void AddEntry(LoggerEntry loggerEntry);
        LoggerEntry GetNextEntry();
    }
}
