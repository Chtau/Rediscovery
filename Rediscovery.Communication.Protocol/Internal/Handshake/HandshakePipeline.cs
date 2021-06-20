using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Handshake
{
    internal class HandshakePipeline : IHandshakePipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;
        private readonly IEncryption _encryption;
        private readonly IDeviceManager _deviceManager;
        private readonly IDiagnosticPackage _diagnosticPackage;
        private readonly ICommunication _communication;

        private string currentIdentifier;
        private Action<AcknowledgeResult> deviceAcknowledgeCallback;

        public HandshakePipeline(IProtocolLogger logger,
            ISerializer serializer,
            IEncryption encryption,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage,
            ICommunication communication)
        {
            _logger = logger;
            _serializer = serializer;
            _encryption = encryption;
            _deviceManager = deviceManager;
            _diagnosticPackage = diagnosticPackage;
            _communication = communication;
            _communication.Receive += Communication_Receive;
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

        public void AcknowledgeCommunication(Action<AcknowledgeResult> acknowledgeCallback)
        {
            deviceAcknowledgeCallback = acknowledgeCallback;
        }

        public void SynchronizeCommunication(DeviceGreetingReceived deviceGreeting)
        {
            try
            {

            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Communication_Receive(object sender, byte[] e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
