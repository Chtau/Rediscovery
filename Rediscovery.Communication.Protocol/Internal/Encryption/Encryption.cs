using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Encryption : IEncryption
    {
        public int SymmetricEncryptionSignatureLength => SymmetricAES.MinimumEncryptedMessageByteSize;
        public Keys RSAKey { get; private set; } 
        public string SymmetricPassword { get; private set; }

        public Encryption()
        {
            RSAKey = GenerateRSA();
        }

        public Keys GenerateRSA()
        {
            var csp = new RSACryptoServiceProvider(4096);
            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);
            return new Keys(privKey.Serialize(), pubKey.Serialize());
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

        public void SetInternRAS(Keys key) => RSAKey = key;
        public void SetInternSymmetric(string password) => SymmetricPassword = password;
    }
}
