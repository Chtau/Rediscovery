using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal class Encryption : IEncryption
    {
        public int SymmetricEncryptionSignatureLength => SymmetricAES.MinimumEncryptedMessageByteSize;
        public string SymmetricPassword { get; private set; }

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

        public void SetInternSymmetric(string password) => SymmetricPassword = password;

        public string CreatePassword(int length = 64, byte[] seed = null) => CryptographyRandomString.GetAlphanumericExtendet(length, seed);
    }
}
