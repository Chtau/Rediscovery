using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal interface IEncryption
    {
        Keys GenerateRSA();
        byte[] EncryptRSA(string publicKey, byte[] content);
        byte[] DecryptRSA(string privateKey, byte[] cypherContent);
        byte[] EncryptAES(string password, byte[] content);
        byte[] DecryptAES(string password, byte[] cypherContent);
    }
}
