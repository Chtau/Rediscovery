using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Encryption : IEncryption
    {
        public string AESPassword { get; private set; }
        public Keys RSAKey { get; private set; }

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

        public byte[] EncryptAES(byte[] content)
        {
            if (!string.IsNullOrWhiteSpace(AESPassword))
                return SymmetricAES.Encrypt(content, AESPassword);
            return content;
        }

        public byte[] DecryptAES(byte[] cypherContent)
        {
            if (!string.IsNullOrWhiteSpace(AESPassword))
                return SymmetricAES.Decrypt(cypherContent, AESPassword);
            return cypherContent;
        }

        public void SetPrivateRASKey(Keys key)
        {
            RSAKey = key;
        }

        public void SetAESPassword(string password)
        {
            AESPassword = password;
        }
    }
}
