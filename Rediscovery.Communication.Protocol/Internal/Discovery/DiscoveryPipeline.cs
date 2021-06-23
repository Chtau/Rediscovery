using Rediscovery.Communication.Protocol.Internal.Encryption;
using Rediscovery.Communication.Protocol.Internal.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Discovery
{
    internal class DiscoveryPipeline : IDiscoveryPipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;
        private readonly INetworkState _networkState;
        private readonly IEncryption _encryption;

        public DiscoveryPipeline(IProtocolLogger logger, ISerializer serializer, INetworkState networkState, IEncryption encryption)
        {
            _logger = logger;
            _serializer = serializer;
            _networkState = networkState;
            _encryption = encryption;
        }

        public T Incoming<T>(byte[] raw)
        {
            T result = default;
            try
            {
                _networkState.EnumerateDecryptPasswords((pw) =>
                {
                    try
                    {
                        var data = _encryption.DecryptSymmetric(pw, raw);
                        result = _serializer.Deserialize<T>(data);
                        if (result != null)
                            return true;
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                    return false;
                });
                return result;
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return default;
        }

        public byte[] Outgoing<T>(T instance)
        {
            try
            {
                var raw = _serializer.Serialize(instance);
                return _networkState.Encrypt(raw);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }
    }
}
