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
            var encryption1 = new Internal.Encryption.Encryption();
            var bytes1 = encryption1.DHKeys();

            var encryption = new Internal.Encryption.Encryption();
            var bytes = encryption.DHKeys();
            var shared = encryption.DHSharedKey(bytes1.Public, bytes.Private);
            Assert.True(shared.Length == 64, "Shared key from new Instance");

            string sharedKey = Convert.ToBase64String(shared);

            var encryption2 = new Internal.Encryption.Encryption();
            var shared2 = encryption2.DHSharedKey(bytes.Public, bytes1.Private);
            Assert.True(shared2.Length == 64, "Shared key with Imported Private key");

            string sharedKey2 = Convert.ToBase64String(shared2);
            Assert.True(sharedKey == sharedKey2, "Shared key values don't match");
        }
    }
}
