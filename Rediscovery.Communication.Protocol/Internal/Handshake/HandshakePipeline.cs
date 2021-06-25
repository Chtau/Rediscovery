using Rediscovery.Communication.Protocol.Internal.Data;
using Rediscovery.Communication.Protocol.Internal.Device;
using Rediscovery.Communication.Protocol.Internal.Diagnostic;
using Rediscovery.Communication.Protocol.Internal.Encryption;
using Rediscovery.Communication.Protocol.Internal.Network;
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
        private readonly INetworkState _networkState;
        private readonly List<AcknowledgeResult> _acknowledgeResults = new List<AcknowledgeResult>();

        private string currentIdentifier;
        private Action<AcknowledgeResult> deviceAcknowledgeCallback;

        public HandshakePipeline(IProtocolLogger logger,
            ISerializer serializer,
            IEncryption encryption,
            IDeviceManager deviceManager,
            IDiagnosticPackage diagnosticPackage,
            ICommunication communication,
            INetworkState networkState)
        {
            _logger = logger;
            _serializer = serializer;
            _encryption = encryption;
            _deviceManager = deviceManager;
            _diagnosticPackage = diagnosticPackage;
            _communication = communication;
            _networkState = networkState;
            _communication.Receive += Communication_Receive;
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier.ExactLength(16);

        public void AcknowledgeCommunication(Action<AcknowledgeResult> acknowledgeCallback)
        {
            deviceAcknowledgeCallback = acknowledgeCallback;
        }

        public void SynchronizeCommunication(DeviceGreetingReceived deviceGreeting)
        {
            try
            {
                var key = _serializer.Serialize(_encryption.RSAKey.Public);
                var rawPackage = new HandshakeState(currentIdentifier,
                    deviceGreeting.Device.Identifier,
                    key.GetChecksum(),
                    key,
                    HandshakeState.MessageValueType.PublicKey,
                    HandshakeState.ExpectedResponseType.SymmetricPasswordCypher);
                var ack = new AcknowledgeResult(deviceGreeting.Device.Identifier, null);
                ack.StartRequest();
                _acknowledgeResults.Add(ack);
                OnSendPackage(rawPackage, deviceGreeting);
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Communication_Receive(object sender, byte[] e)
        {
            HandshakeState pack = null;
            try
            {
                // if we receive a valid package we need to check that we are the receiver
                _networkState.EnumerateDecryptPasswords((pw) =>
                {
                    try
                    {
                        pack = new HandshakeState(_serializer.Deserialize<byte[]>(_encryption.DecryptSymmetric(pw, e)));
                        if (pack.IsValid() && pack.ReceiverIdentifier == currentIdentifier)
                            return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                    return false;
                });
                if (pack != null && pack.IsValid())
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
                        // response to our request
                        ack.ResponseReceived(AcknowledgeResult.State.Ok, pack);
                        deviceAcknowledgeCallback?.Invoke(ack);
                        _acknowledgeResults.Remove(ack);
                    } else
                    {
                        // we need to response
                        OnHandleAckResponse(pack.SenderIdentifier, pack.ResponseType, pack.Value);
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

        private void OnHandleAckResponse(string senderIdentifer, HandshakeState.ExpectedResponseType expectedResponseType, byte[] value)
        {
#if PIPELINE
            _logger.Trace($"{nameof(HandshakePipeline)}.{nameof(OnHandleAckResponse)} device:\"{senderIdentifer}\" expecte a response for {nameof(HandshakeState.ExpectedResponseType)}:{Enum.GetName(typeof(HandshakeState.ExpectedResponseType), expectedResponseType)}");
#endif
            HandshakeState package = null;
            var deviceGreeting = _deviceManager.GetGreeting(senderIdentifer);
            switch (expectedResponseType)
            {
                case HandshakeState.ExpectedResponseType.PublicKey:
                    var key = _serializer.Serialize(_encryption.RSAKey.Public);
                    package = new HandshakeState(currentIdentifier,
                        deviceGreeting.Device.Identifier,
                        key.GetChecksum(),
                        key,
                        HandshakeState.MessageValueType.PublicKey,
                        HandshakeState.ExpectedResponseType.None);
                    break;
                case HandshakeState.ExpectedResponseType.SymmetricPasswordCypher:
                    var pubKey = _serializer.Deserialize<string>(value);
                    var plainPW = _serializer.Serialize(_encryption.SymmetricPassword);
                    var encPW = _encryption.EncryptRSA(pubKey, plainPW);
                    var pw = (encPW);
                    package = new HandshakeState(currentIdentifier,
                        deviceGreeting.Device.Identifier,
                        pw.GetChecksum(),
                        pw,
                        HandshakeState.MessageValueType.SymmetricPasswordCypher,
                        HandshakeState.ExpectedResponseType.None);
                    break;
                case HandshakeState.ExpectedResponseType.None:
                default:
                    // done
                    break;
            }
            if (package != null)
                OnSendPackage(package, deviceGreeting);
        }

        private void OnSendPackage(HandshakeState package, DeviceGreetingReceived deviceGreeting)
        {
            var raw = _serializer.Serialize(package.CreateRaw().ToArray());
            var enc = _networkState.Encrypt(_networkState.NormalizePackageSize(raw, deviceGreeting.Device.Communication.Handshake.PackageSize));
            _communication.Send(new TCPCommunicationPayload(enc,
                deviceGreeting.Device.Identifier,
                deviceGreeting.Device.Communication.Handshake.Port,
                deviceGreeting.Device.Communication.Handshake.PackageSize));
        }
    }
}
