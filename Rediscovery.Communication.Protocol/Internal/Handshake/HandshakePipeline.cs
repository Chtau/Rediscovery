using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<AcknowledgeResult> _acknowledgeResults = new List<AcknowledgeResult>();

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

        public void SynchronizeCommunication(DeviceGreetingReceived deviceGreeting, string password)
        {
            try
            {
                var key = _encryption.RSAKey.Public;
                var rawPackage = new HandshakeState(currentIdentifier,
                    deviceGreeting.Device.Identifier,
                    key.GetChecksum(),
                    Convert.FromBase64String(key),
                    HandshakeState.MessageValueType.PublicKey,
                    HandshakeState.ExpectedResponseType.SymmetricPasswordCypher);
                var ack = new AcknowledgeResult(deviceGreeting.Device.Identifier, password);
                ack.StartRequest();
                _acknowledgeResults.Add(ack);
                // configuration for a handshake (default) password 
                _communication.Send(new CommunicationPayload(_encryption.EncryptSymmetric(ack.HandshakePassword, _serializer.Serialize(rawPackage)), deviceGreeting.Device.Identifier));
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Communication_Receive(object sender, byte[] e)
        {
            try
            {
                // TODO: check against all acknowledge where the password gives a valid package
                //       if we receive a valid package we need to check that we are the receiver
                var pack = new HandshakeState(_encryption.DecryptSymmetric("", e));
                if (pack.IsValid())
                {
                    if (pack.ReceiverIdentifier != currentIdentifier)
                    {
#if PIPELINE
                        _logger.Trace($"{nameof(HandshakePipeline)}.{nameof(Communication_Receive)} Put we're not the correct receiver in the package. (Content:{pack})");
#endif
                    }
#if PIPELINE
                    _logger.Trace($"{nameof(HandshakePipeline)}.{nameof(Communication_Receive)} Content:{pack}");
#endif
                    var ack = _acknowledgeResults.FirstOrDefault(x => x.RemoteDeviceIdentifer == pack.SenderIdentifier);
                    if (ack != null)
                    {
                        ack.ResponseReceived(AcknowledgeResult.State.Ok);
                        deviceAcknowledgeCallback?.Invoke(ack);
                        _acknowledgeResults.Remove(ack);
                    }
                }
                else
                {
#if PIPELINE
                    _logger.Warning($"{nameof(HandshakePipeline)}.{nameof(Communication_Receive)} package is not valid. (Content:\"{pack}\"");
#endif
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
