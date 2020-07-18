using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CommunicationLoggerConsumer
{
    public interface ILoggerConsumer
    {
        bool IsConnect { get; }
        bool Connect(string ipAddress, int port, string certificatePEM);
        bool Disconnect();
        void StartLogger(string token, CancellationTokenSource cts = null);
        void LogEntry(SharedBase.Logging.LoggerEntry loggerEntry);
    }
}
