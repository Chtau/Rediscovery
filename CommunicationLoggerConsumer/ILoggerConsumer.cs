using Rediscovery.Communication.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Communication.Logger.Consumer
{
    public interface ILoggerConsumer
    {
        event EventHandler<Rediscovery.Shared.Base.Logging.LogCommandConfigResult> LoggerCommandExecuted;

        bool IsConnect { get; }
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();
        void StartLogger(string token, CancellationTokenSource cts = null);
        void LogEntry(Rediscovery.Shared.Base.Logging.LoggerEntry loggerEntry);
        void LoggerCommand(string token, Rediscovery.Shared.Base.Logging.LogCommandConfig logCommandConfig);
    }
}
