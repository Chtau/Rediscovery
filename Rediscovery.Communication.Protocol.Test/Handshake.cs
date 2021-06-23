using Rediscovery.Communication.Protocol.Internal.Handshake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class Handshake
    {
        [Fact]
        public async void InOutLoopBack()
        {
            IProtocolLogger logger = new Internal.ProtocolLogger();
            Internal.Device.IDeviceManager deviceManager = new Internal.Device.DeviceManager(logger);
            Internal.Diagnostic.IDiagnosticPackage diagnosticPackage = new Internal.Diagnostic.DiagnosticPackage(logger);
            Internal.Encryption.IEncryption encryption = new Internal.Encryption.Encryption();
            IHandshakePipeline handshakePipeline = new HandshakePipeline(logger,
                new Internal.Serializer(logger),
                encryption,
                deviceManager,
                diagnosticPackage,
                new Mocks.Communication(),
                new Internal.Network.NetworkState(logger, encryption));
            var device = new Models.DeviceGreeting
            {
                FriendlyName = "A",
                Identifier = "3C07A55EDA88491C9A84C469C19E4F44",
                Hops = 0,
                Metadata = new Models.DeviceMetadata
                {
                    Idiom = Models.DeviceMetadata.IdiomType.Undefined
                }
            };
            AcknowledgeResult ackResult = null;
            handshakePipeline.SetIdentifier("3C07A55EDA88491C9A84C469C19E4F44");
            handshakePipeline.AcknowledgeCommunication((ack) =>
            {
                ackResult = ack;
            });
            handshakePipeline.SynchronizeCommunication(new Internal.Device.DeviceGreetingReceived(device, "127.0.0.1"));
            await Task.Delay(TimeSpan.FromSeconds(1));
            Assert.NotNull(ackResult);
            Assert.True(ackResult.ResponseState == AcknowledgeResult.State.Ok);
        }

        [Fact]
        public async void In()
        {
            IProtocolLogger logger = new Internal.ProtocolLogger();
            Internal.Device.IDeviceManager deviceManager = new Internal.Device.DeviceManager(logger);
            Internal.Diagnostic.IDiagnosticPackage diagnosticPackage = new Internal.Diagnostic.DiagnosticPackage(logger);
            Internal.Encryption.IEncryption encryption = new Internal.Encryption.Encryption();
            IHandshakePipeline handshakePipeline = new HandshakePipeline(logger,
                new Internal.Serializer(logger),
                encryption,
                deviceManager,
                diagnosticPackage,
                new Mocks.Communication(),
                new Internal.Network.NetworkState(logger, encryption));
            var device = new Models.DeviceGreeting
            {
                FriendlyName = "A",
                Identifier = "06E99EC39D7549F8969484409C24EFC7",
                Hops = 0,
                Metadata = new Models.DeviceMetadata
                {
                    Idiom = Models.DeviceMetadata.IdiomType.Undefined
                }
            };
            AcknowledgeResult ackResult = null;
            handshakePipeline.SetIdentifier("3C07A55EDA88491C9A84C469C19E4F44");
            handshakePipeline.AcknowledgeCommunication((ack) =>
            {
                ackResult = ack;
            });
            handshakePipeline.SynchronizeCommunication(new Internal.Device.DeviceGreetingReceived(device, "127.0.0.1"));
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.Null(ackResult);
        }
    }
}
