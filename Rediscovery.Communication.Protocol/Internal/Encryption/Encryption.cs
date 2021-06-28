using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Encryption : IEncryption
    {
        public int SymmetricEncryptionSignatureLength => SymmetricAES.MinimumEncryptedMessageByteSize;
        public Keys<string> RSAKey { get; private set; } 
        public string SymmetricPassword { get; private set; }

        public Encryption()
        {
            RSAKey = GenerateRSA();
        }

        public Keys<string> GenerateRSA()
        {
            var csp = new RSACryptoServiceProvider(4096);
            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);
            return new Keys<string>(privKey.Serialize(), pubKey.Serialize());
        }

        public byte[] EncryptRSA(string publicKey, byte[] content)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(publicKey.DeserializeRSA());
            // apply pkcs#1.5 padding and encrypt
            return csp.Encrypt(content, false);
        }

        public byte[] DecryptRSA(string privateKey, byte[] cypherContent)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey.DeserializeRSA());
            // decrypt and strip pkcs#1.5 padding
            return csp.Decrypt(cypherContent, false);
        }

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

        public void SetInternRAS(Keys<string> key) => RSAKey = key;

        public void SetInternSymmetric(string password) => SymmetricPassword = password;

        public string CreatePassword(int length = 64) => CryptographyRandomString.GetAlphanumericExtendet(length);

        public Keys<byte[]> DHKeys()
        {
            using ECDiffieHellmanCng client = new ECDiffieHellmanCng();
            client.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
            client.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
            var pKey = client.ExportECPrivateKey();
            return new Keys<byte[]>(pKey, client.PublicKey.ToByteArray());
        }

        public byte[] DHSharedKey(byte[] remotePublicKey)
        {
            using ECDiffieHellmanCng client = new ECDiffieHellmanCng();
            client.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hmac;
            client.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP521;
            var pbkey = ECDiffieHellmanCngPublicKey.FromByteArray(remotePublicKey, CngKeyBlobFormat.EccPublicBlob);
            return client.DeriveKeyFromHmac(pbkey, HashAlgorithmName.SHA512, null, null, null);
        }
    }
}
