using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerConsumer
{
    public interface ILoggerConsumer
    {
        bool Connect(string ipAddress, int port, string certificatePEM, string authorizationToken);
        bool Disconnect();
        void LogEntry(SharedBase.Logging.LoggerEntry loggerEntry);
    }
}
