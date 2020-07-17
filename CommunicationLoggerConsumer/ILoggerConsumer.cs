using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationLoggerConsumer
{
    public interface ILoggerConsumer
    {
        bool Connect(string ipAddress, int port, string certificatePEM);
        bool Disconnect();
        void LogEntry(string token, SharedBase.Logging.LoggerEntry loggerEntry);
    }
}
