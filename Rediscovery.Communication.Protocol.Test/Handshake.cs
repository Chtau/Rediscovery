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
            var device2 = new Models.DeviceGreeting
            {
                FriendlyName = "B",
                Identifier = "BC07A55EDA88491C9A84C469C19E4F44",
                Hops = 0,
                Metadata = new Models.DeviceMetadata
                {
                    Idiom = Models.DeviceMetadata.IdiomType.Undefined
                },
                Communication = new Models.DeviceCommunication
                {
                    Handshake = new Models.DeviceCommunicationSetting()
                }
            };
            deviceManager.Change(device2, System.Net.IPEndPoint.Parse("127.0.0.1"));

            Internal.Device.IDeviceManager deviceManager2 = new Internal.Device.DeviceManager(logger);
            var device = new Models.DeviceGreeting
            {
                FriendlyName = "A",
                Identifier = "3C07A55EDA88491C9A84C469C19E4F44",
                Hops = 0,
                Metadata = new Models.DeviceMetadata
                {
                    Idiom = Models.DeviceMetadata.IdiomType.Undefined
                },
                Communication = new Models.DeviceCommunication
                {
                    Handshake = new Models.DeviceCommunicationSetting()
                }
            };
            deviceManager2.Change(device, System.Net.IPEndPoint.Parse("127.0.0.1"));

            Internal.Diagnostic.IDiagnosticPackage diagnosticPackage = new Internal.Diagnostic.DiagnosticPackage(logger);
            Internal.Encryption.IEncryption encryption = new Internal.Encryption.Encryption();

            var com1 = new Mocks.Communication();
            var com2 = new Mocks.Communication();
            com1.MockSend += (obj, args) =>
            {
                com2.InvokeReceive(args);
            };
            com2.MockSend += (obj, args) =>
            {
                com1.InvokeReceive(args);
            };

            IHandshakePipeline handshakePipeline = new HandshakePipeline(logger,
                new Internal.Serializer(logger),
                encryption,
                deviceManager,
                diagnosticPackage,
                com1,
                new Internal.Network.NetworkState(logger, encryption));

            IHandshakePipeline handshakePipeline2 = new HandshakePipeline(logger,
                new Internal.Serializer(logger),
                encryption,
                deviceManager2,
                diagnosticPackage,
                com2,
                new Internal.Network.NetworkState(logger, encryption));

            

            handshakePipeline2.SetIdentifier("BC07A55EDA88491C9A84C469C19E4F44");
            handshakePipeline2.AcknowledgeCommunication((ack) =>
            {
                var ackResult1 = ack;
            });

            AcknowledgeResult ackResult = null;
            handshakePipeline.SetIdentifier("3C07A55EDA88491C9A84C469C19E4F44");
            handshakePipeline.AcknowledgeCommunication((ack) =>
            {
                ackResult = ack;
            });
            handshakePipeline.SynchronizeCommunication(new Internal.Device.DeviceGreetingReceived(device2, "127.0.0.1"));
            await Task.Delay(TimeSpan.FromSeconds(10));
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
