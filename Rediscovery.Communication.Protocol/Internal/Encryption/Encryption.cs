using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Encryption : IEncryption
    {
        // TODO: Need to replace RSACryptoServiceProvider because of PlatformNotSupported in Blazor => replcae with Diffie-Hellman
        // TODO: Need to replace SymmetricAES.Encrypt because of PlatformNotSupported in Blazor => Need Implementation
        // TODO: Need to replace CryptographyRandomString.GetAlphanumericExtendet (System.Security.Cryptography.Csp) because of PlatformNotSupported in Blazor => Not used at the moment (only nice to have)


        public int SymmetricEncryptionSignatureLength => SymmetricAES.MinimumEncryptedMessageByteSize;
        //public Keys<string> RSAKey { get; private set; }
        public string SymmetricPassword { get; private set; }

        public Encryption()
        {
            //RSAKey = GenerateRSA();
            //var a = SymmetricAES.Encrypt(System.Text.Encoding.UTF8.GetBytes("testetst"), "asdasdasd");
            //var pw = CryptographyRandomString.GetAlphanumericExtendet(128);
        }

        /*public Keys<string> GenerateRSA()
        {
            var csp = new RSACryptoServiceProvider(4096);
            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);
            return new Keys<string>(privKey.Serialize(), pubKey.Serialize());
        }*/

        /*public byte[] EncryptRSA(string publicKey, byte[] content)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(publicKey.DeserializeRSA());
            // apply pkcs#1.5 padding and encrypt
            return csp.Encrypt(content, false);
        }*/

        /*public byte[] DecryptRSA(string privateKey, byte[] cypherContent)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey.DeserializeRSA());
            // decrypt and strip pkcs#1.5 padding
            return csp.Decrypt(cypherContent, false);
        }*/

        public byte[] EncryptSymmetric(string password, byte[] content)
        {
            if (!string.IsNullOrWhiteSpace(password))
                return SymmetricAES.Encrypt(content, password);
            return content;
        }

        public byte[] DecryptSymmetric(string password, byte[] cypherContent)
        {
            if (!string.IsNullOrWhiteSpace(password))
                return SymmetricAES.Decrypt(cypherContent, password);
            return cypherContent;
        }

        //public void SetInternRAS(Keys<string> key) => RSAKey = key;

        public void SetInternSymmetric(string password) => SymmetricPassword = password;

        public string CreatePassword(int length = 64, byte[] seed = null) => CryptographyRandomString.GetAlphanumericExtendet(length, seed);

        /*public Keys<byte[]> DHKeys()
        {
            using ECDiffieHellmanCng client = new ECDiffieHellmanCng();
            client.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
            client.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
            var pKey = client.ExportECPrivateKey();
            return new Keys<byte[]>(pKey, client.PublicKey.ToByteArray());
        }*/

        /*public byte[] DHSharedKey(byte[] remotePublicKey, byte[] privateKey)
        {
            using ECDiffieHellmanCng client = new ECDiffieHellmanCng();
            client.ImportECPrivateKey(privateKey, out int read);
            client.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
            client.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
            var pbkey = ECDiffieHellmanCngPublicKey.FromByteArray(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            return client.DeriveKeyFromHmac(pbkey, HashAlgorithmName.SHA512, null, null, null);
        }*/
    }
}
