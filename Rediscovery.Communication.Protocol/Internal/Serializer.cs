using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal class Serializer : ISerializer
    {
        private readonly IProtocolLogger _logger;

        public Serializer(IProtocolLogger logger)
        {
            _logger = logger;
        }

        public T Deserialize<T>(byte[] raw)
        {
            try
            {
                return (T)MessagePackSerializer.Typeless.Deserialize(raw, OnGetOptions());
            } catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return default;
        }

        public byte[] Serialize<T>(T instance)
        {
            try
            {
                return MessagePackSerializer.Typeless.Serialize(instance, OnGetOptions());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return null;
        }

        private MessagePackSerializerOptions OnGetOptions()
        {
            return MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4BlockArray);
        }
    }
}
