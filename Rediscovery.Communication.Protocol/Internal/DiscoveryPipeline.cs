using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class DiscoveryPipeline : IDiscoveryPipeline
    {
        private readonly IProtocolLogger _logger;
        private readonly ISerializer _serializer;

        public DiscoveryPipeline(IProtocolLogger logger, ISerializer serializer)
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

        public byte[] Outgoing<T>(T instance)
        {
            try
            {
                return _serializer.Serialize(instance);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }
    }
}
