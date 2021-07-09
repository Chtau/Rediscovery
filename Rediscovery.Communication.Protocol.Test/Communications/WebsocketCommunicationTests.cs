using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test.Communications
{
    public class WebSocketCommunicationTests
    {
        [Fact]
        public async void Send()
        {
            IProtocolLogger logger = new Internal.ProtocolLogger();
            Internal.Diagnostic.IDiagnosticPackage diagnosticPackage = new Internal.Diagnostic.DiagnosticPackage(logger);
            Internal.Encryption.IEncryption encryption = new Internal.Encryption.Encryption();
            ISerializer serializer = new Internal.Serializer(logger);
            Internal.Device.IDeviceManager deviceManager = new Internal.Device.DeviceManager(logger, encryption, serializer);
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
            deviceManager.SetIdentifier("3C07A55EDA88491C9A84C469C19E4F44");

            var server = new Internal.Data.WebSocketServerCommunication(logger, deviceManager, diagnosticPackage, encryption);
            Task.Run(() => server.OnOpenWebSocket());

            var com = new Internal.Data.WebSocketCommunication(logger, deviceManager, diagnosticPackage, encryption);
            com.Initialize(new Models.ConnectionListenConfiguration
            {
                Port = 49889,
                PackageSize = 1024
            });
            com.Start();
            com.Receive += (obj, args) =>
            {
                var receivedAsText = Encoding.UTF8.GetString(args, 0, args.Length);
                logger.Trace("Client Received:" + receivedAsText);
                com.Stop();
            };
            var payload = Encoding.UTF8.GetBytes("Hallo");
            com.Send(new Internal.Data.PortCommunicationPayload(payload, "BC07A55EDA88491C9A84C469C19E4F44", 49889, 1024));

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}
