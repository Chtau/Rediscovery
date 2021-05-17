using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IRediscoveryProtocol
    {
        void Start(int? listenPort);
        ConnectionState Connect(Connection connection);
        bool Disconnect();
        TransportState Send(Transfer transfer);
        TransportState Stream(Action<object> streamData);
        void Listen(Action<Transfer> receivedCallback);
        TransportState LowLatencySend(Transfer transfer);
        TransportState LowLatencyStream(Action<object> streamData);
        void LowLatencyListen(Action<Transfer> receivedCallback);
        object GetDiagnosticData();
        object GetConnectionInfo();
    }
}
