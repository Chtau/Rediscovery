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
        private readonly byte valueDelimiter = Encoding.UTF8.GetBytes("+").First();

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
                //var headerBytes = new List<byte>(46); // Convert.FromBase64String => 12 + 12 + 6 + 7 + 12 + 3
                var localDeviceBytes = Convert.FromBase64String(currentIdentifier); // 12 byte = sender device (local)
                var remoteDeviceBytes = Convert.FromBase64String(deviceGreeting.Device.Identifier); // 12 byte = receiver device (remote)
                
                var rawPayload = _serializer.Serialize(instance);
                var payloadSize = rawPayload.Length;
                var payloadSizeBytes = Encoding.UTF8.GetBytes($"+{payloadSize}+"); // ?? byte = length of the total payload
                var checksum = rawPayload.GetHashString(HashExtensions.HashAlgorithmTypes.MD5).Substring(0, 16);
                var checksumBytes = Convert.FromBase64String(checksum); // 12 byte = checksum MD5 first 16 characters (is at the same time the overall package identifier)
                
                var packSize = deviceGreeting.Device.Communication.Data.PackageSize;
                var headerPackSize = (packSize + headerSize);
                var packCount = payloadSize / headerPackSize;
                if (packCount == 0)
                    packCount = 1;

                var packs = new PackagePartState[packCount];
                for (int i = 0; i < packCount; i++)
                {
                    var headerBytes = new List<byte>(46);
                    headerBytes.AddRange(localDeviceBytes);
                    headerBytes.AddRange(remoteDeviceBytes);
                    headerBytes.AddRange(Encoding.UTF8.GetBytes($"+{i}+")); // ?? byte = package index
                    headerBytes.AddRange(Convert.FromBase64String(DateTime.UtcNow.ToString("mmssffff"))); // 7 byte = sender timestamp format "minutes-seconds-tousends of second"

                    packs[i] = new PackagePartState
                    {
                        PartPayload = rawPayload.Skip(headerPackSize * i).Take(headerPackSize).ToArray(),
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
