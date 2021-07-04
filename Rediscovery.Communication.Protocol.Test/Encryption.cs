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
        [Theory]
        [InlineData("6BB0C84B-00CD-41FE-AF33-F2038ADC294C", "F7D86692-7089-477D-9315-FB3392757834")]
        [InlineData("{DBD8C3AB-87FC-406A-97D4-135CFEAD46F2}", "{729C5D04-4835-4970-A476-84C99ACFF581}")]
        [InlineData("{1CAB404D-8D2E-43AF-8484-0FAFD342F8E5}", "{47E9FB7D-A69C-4A43-9F36-6F3279549FCB}")]
        [InlineData("{CB10169C-C680-4AF6-B5AD-4F9FD2631340}", "{37BA92C8-94B5-4A39-97D5-F039CCC51B1C}")]
        [InlineData("{1C3BA6EF-35CC-4A92-9145-CCEC89768BE0}", "{16FAF50D-409D-4E6C-B492-C62B01D73F3A}")]
        [InlineData("{9F866A95-F78B-4931-BBEC-A1DFD41F480E}", "{F89BF020-0EBE-4CCC-AB07-701B2A76C1FB}")]
        [InlineData("{A529F099-3274-483F-8314-0610E34BABD3}", "{F89BF020-0EBE-4CCC-AB07-701B2A76C1FB}")]
        [InlineData("{0870337D-DDD4-4B5F-A537-AA4CE6255B80}", "{F89BF020-0EBE-4CCC-AB07-701B2A76C1FB}")]
        public void Symmetric(string pw, string dataRaw)
        {
            var dataByte = Encoding.UTF8.GetBytes(dataRaw);
            var encryption = new Internal.Encryption.Encryption();
            var cypher = encryption.EncryptSymmetric(pw, dataByte);
            var dataByteOut = encryption.DecryptSymmetric(pw, cypher);
            string dataRawOut = Encoding.UTF8.GetString(dataByteOut);
            Assert.True(dataRaw == dataRawOut, "String data output");
            Assert.True(Convert.ToBase64String(dataByte) == Convert.ToBase64String(dataByteOut), "Byte data output");
        }

        [Theory]
        [InlineData(true, "{3F320172-C697-409F-8132-AC4642B43A1D}", "{191F7601-5B52-4737-99D7-68EC964B2B85}")]
        [InlineData(true, null, "{191F7601-5B52-4737-99D7-68EC964B2B85}")]
        [InlineData(true, "{3F320172-C697-409F-8132-AC4642B43A1D}", null)]
        [InlineData(true, null, null)]
        [InlineData(false, "{3F320172-C697-409F-8132-AC4642B43A1D}", "{191F7601-5B52-4737-99D7-68EC964B2B85}")]
        [InlineData(false, null, "{191F7601-5B52-4737-99D7-68EC964B2B85}")]
        [InlineData(false, "{3F320172-C697-409F-8132-AC4642B43A1D}", null)]
        [InlineData(false, null, null)]
        public void DiffieHellmanImplementation(bool importKey, string keySeed, string curveSeed)
        {
            byte[] keyS = null;
            if (!string.IsNullOrWhiteSpace(keySeed))
                keyS = Encoding.UTF8.GetBytes(keySeed);
            byte[] curveS = null;
            if (!string.IsNullOrWhiteSpace(curveSeed))
                curveS = Encoding.UTF8.GetBytes(curveSeed);

            var pairAlice = AsymmetricDiffieHellman.GetNewKeyPair(keyS);
            var pairBob = AsymmetricDiffieHellman.GetNewKeyPair(keyS);

            var dhAlice = new AsymmetricDiffieHellman(curveS);
            if (importKey)
                dhAlice.ImportKeyPair(pairAlice);
            else
                dhAlice.CreateKeyPair(keyS);

            var dhBob = new AsymmetricDiffieHellman(curveS);
            if (importKey)
                dhBob.ImportKeyPair(pairBob);
            else
                dhBob.CreateKeyPair(keyS);


            dhAlice.SetPublicKey(dhBob.LocalPublicKey);
            var sharedAlice = dhAlice.GetSharedSecret();
            Assert.True(sharedAlice.Length == 64, $"Alice: Shared key length:{sharedAlice.Length}");

            dhBob.SetPublicKey(dhAlice.LocalPublicKey);
            var sharedBob = dhBob.GetSharedSecret();
            Assert.True(sharedBob.Length == 64, $"Bob: Shared key length:{sharedBob.Length}");

            string sharedKeyAlice = Convert.ToBase64String(sharedAlice);
            string sharedKeyBob = Convert.ToBase64String(sharedBob);
            var lAlice = sharedKeyAlice.Length;
            var lBob = sharedKeyBob.Length;
            Assert.True(sharedKeyAlice == sharedKeyBob, "Shared key value match");
        }
    }
}
