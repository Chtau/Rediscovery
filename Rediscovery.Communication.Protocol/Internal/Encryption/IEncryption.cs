using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal interface IEncryption
    {
        Keys RSAKey { get; }
        void SetPrivateRASKey(Keys key);
        void SetAESPassword(string password);
        Keys GenerateRSA();
        byte[] EncryptRSA(string publicKey, byte[] content);
        byte[] DecryptRSA(string privateKey, byte[] cypherContent);
        byte[] EncryptAES(byte[] content);
        byte[] DecryptAES(byte[] cypherContent);
    }
}
