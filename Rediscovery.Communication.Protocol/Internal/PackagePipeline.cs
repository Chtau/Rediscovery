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
        
        private string currentIdentifier;

        public event EventHandler<OutgoingPackageRawPart> SendNextRaw;

        public PackagePipeline(IProtocolLogger logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
        }

        public void SetIdentifier(string identifier) => currentIdentifier = identifier;

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
                var rawPayload = _serializer.Serialize(instance).ToList();
                var payloadSize = rawPayload.Count;
                var checksum = rawPayload.ToArray().GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0, 16);
                
                var packSize = deviceGreeting.Device.Communication.Data.PackageSize;

                var packs = new List<PackagePartState>();
                var index = 0;
                while (rawPayload.Count > 0)
                {
                    // remove used bytes from raw payload when added to packs
                    var pack = new PackagePartState(packSize,
                        currentIdentifier,
                        deviceGreeting.Device.Identifier,
                        checksum,
                        payloadSize,
                        index);
                    var headerSize = pack.HeaderSizeOnly();
                    pack.SetPayload(rawPayload.Take(packSize - headerSize).ToArray());
                    rawPayload.RemoveRange(0, packSize - headerSize);
                    index++;
                    packs.Add(pack);
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
