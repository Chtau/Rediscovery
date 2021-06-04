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
