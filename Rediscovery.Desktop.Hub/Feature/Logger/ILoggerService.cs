using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rediscovery.Desktop.Hub.Feature.Logger
{
    public interface ILoggerService
    {
        event EventHandler<LoggerEntryModel> LoggerDataReceived;
        void Init();
    }
}
