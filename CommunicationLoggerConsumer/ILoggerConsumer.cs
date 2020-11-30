using Rediscovery.Communication.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rediscovery.Communication.Consumer.Logger
{
    public interface ILoggerConsumer
    {
        event EventHandler<Rediscovery.Shared.Logging.Commands.LogCommandConfigResult> LoggerCommandExecuted;

        bool IsConnect { get; }
        bool Connect(ConsumerConnectionConfiguration connectionConfiguration);
        bool Disconnect();
        void StartLogger(string token, CancellationTokenSource cts = null);
        void LogEntry(Rediscovery.Shared.Logging.Models.LoggerEntry loggerEntry);
        void LoggerCommand(string token, Rediscovery.Shared.Logging.Commands.LogCommandConfig logCommandConfig);
    }
}
