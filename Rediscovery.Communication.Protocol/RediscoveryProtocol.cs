using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol
{
    public class RediscoveryProtocol : IRediscoveryProtocol
    {
        private readonly IProtocolLogger _protocolLogger;

        public RediscoveryProtocol(IProtocolLogger protocolLogger = null)
        {
            _protocolLogger = protocolLogger ?? new Internal.ProtocolLogger();
        }

        public ConnectionState Connect(Connection connection)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect()
        {
            throw new NotImplementedException();
        }

        public object GetConnectionInfo()
        {
            throw new NotImplementedException();
        }

        public object GetDiagnosticData()
        {
            throw new NotImplementedException();
        }

        public void Listen(Action<Transfer> receivedCallback)
        {
            throw new NotImplementedException();
        }

        public void LowLatencyListen(Action<Transfer> receivedCallback)
        {
            throw new NotImplementedException();
        }

        public TransportState LowLatencySend(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        public TransportState LowLatencyStream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }

        public TransportState Send(Transfer transfer)
        {
            throw new NotImplementedException();
        }

        public TransportState Stream(Action<object> streamData)
        {
            throw new NotImplementedException();
        }
    }
}
