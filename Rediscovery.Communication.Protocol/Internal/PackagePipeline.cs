using Rediscovery.Communication.Protocol.Internal.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class PackagePipeline : IPackagePipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;
        private readonly int headerSize = 0;

        public event EventHandler<OutgoingPackageRawPart> SendNextRaw;

        public PackagePipeline(IProtocolLogger logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public T Incoming<T>(byte[] raw)
        {
            try
            {
                return _serializer.Deserialize<T>(raw);
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return default;
        }

        public bool Outgoing<T>(T instance, DeviceGreetingReceived deviceGreeting)
        {
            try
            {
                var p1 = Convert.FromBase64String("45796974E2128A89"); // 12 byte = sender device (local)
                var p2 = Convert.FromBase64String(deviceGreeting.Device.Identifier); // 12 byte = receiver device (remote)
                var p3 = Convert.FromBase64String(DateTime.UtcNow.ToString("mmssffff")); // ?? byte = sender timestamp format "minutes-seconds-tousends of second"
                var p4 = Encoding.UTF8.GetBytes($"+{64000}+"); // ?? byte = length of the total payload
                var p5 = Convert.FromBase64String("54696974E2128A89"); // ?? byte = checksum MD5 first 16 characters (is at the same time the overall package identifier)
                var p6 = Encoding.UTF8.GetBytes($"+{00000}+"); // ?? byte = package index
                // Encoding.UTF8.GetBytes => 16 + 16 + 8 + 7 + 16 + + 3
                // Convert.FromBase64String => 12 + 12 + 6 + 7 + 12 + 3

                var rawPayload = _serializer.Serialize(instance);
                var payloadSize = rawPayload.Length;
                var packSize = deviceGreeting.Device.Communication.Data.PackageSize;
                var headerPackSize = (packSize + headerSize);
                var packCount = payloadSize / headerPackSize;
                if (packCount == 0)
                    packCount = 1;
                var packs = new PackageState[packCount];
                for (int i = 0; i < packCount; i++)
                {
                    packs[i] = new PackageState
                    {
                        Payload = rawPayload.Skip(headerPackSize * i).Take(headerPackSize).ToArray(),
                        PayloadSize = rawPayload.Length,
                        ReceiverIdentifier = deviceGreeting.Device.Identifier,
                    };
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return false;
        }
    }
}
