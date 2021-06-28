using Rediscovery.Communication.Protocol.Internal.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rediscovery.Communication.Protocol.Test
{
    public class Encryption
    {
        [Fact]
        public void DiffieHellmanPublicKey()
        {
            var encryption = new Internal.Encryption.Encryption();
            var bytes = encryption.DHKeys();
            Assert.True(bytes.Public.Length == 140);
        }

        [Fact]
        public void DiffieHellmanSharedKey()
        {
            var encryption = new Internal.Encryption.Encryption();
            var bytes = encryption.DHKeys();
            var shared = encryption.DHSharedKey(bytes.Public);
            Assert.True(shared.Length == 64);
        }
    }
}
