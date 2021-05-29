using Rediscovery.Communication.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public interface IRediscoveryProtocol
    {
        List<DeviceGreeting> Devices { get; }
        event EventHandler DevicesChanged;

        void Start(Models.Configuration configuration);
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
        void ChangeGreeting(DeviceGreeting greeting);
        void SetIdentifier(string identifer);
        string NewIdentifier();
    }
}
