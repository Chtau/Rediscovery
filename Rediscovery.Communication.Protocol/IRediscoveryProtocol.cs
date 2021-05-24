using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IRediscoveryProtocol
    {
        void Start(Setting setting);
        ConnectionState Connect(Connection connection);
        bool Disconnect();
        void NewDevices(Action<object> deviceCallback);
        void Send(Transfer transfer, Action<TransportState> successCallback = null);
        TransportState Stream(Action<object> streamData);
        void Listen(Action<Transfer> receivedCallback);
        TransportState LowLatencySend(Transfer transfer);
        TransportState LowLatencyStream(Action<object> streamData);
        void LowLatencyListen(Action<Transfer> receivedCallback);
        object GetDiagnosticData();
        object GetConnectionInfo();
    }
}
