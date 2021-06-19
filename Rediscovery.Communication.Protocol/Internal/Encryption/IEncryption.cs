using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal interface IEncryption
    {
        /// <summary>
        /// Access to the current RSA key pair
        /// </summary>
        Keys RSAKey { get; }
        /// <summary>
        /// Sets RSA public and private keys.
        /// </summary>
        /// <param name="key">RSA key pair</param>
        void SetPrivateRASKey(Keys key);
        /// <summary>
        /// Generate new RSA key pair
        /// </summary>
        /// <returns>RSA public and private key</returns>
        Keys GenerateRSA();
        /// <summary>
        /// Create cypher <see cref="byte[]"/> from the raw <see cref="byte[]"/> content.
        /// </summary>
        /// <param name="publicKey">RSA public key</param>
        /// <param name="content">RAW content</param>
        /// <returns>Cypher content</returns>
        byte[] EncryptRSA(string publicKey, byte[] content);
        /// <summary>
        /// Create plain <see cref="byte[]"/> from cypher content.
        /// </summary>
        /// <param name="privateKey">RSA private key</param>
        /// <param name="cypherContent">Cypher content</param>
        /// <returns>Plain content</returns>
        byte[] DecryptRSA(string privateKey, byte[] cypherContent);
        /// <summary>
        /// Create cypher <see cref="byte[]"/> from the raw <see cref="byte[]"/> content.
        /// </summary>
        /// <param name="password">AES password string</param>
        /// <param name="content">RAW content</param>
        /// <returns>Cypher content</returns>
        byte[] EncryptAES(string password, byte[] content);
        /// <summary>
        /// Creates plain <see cref="byte[]"/> from cypher content.
        /// </summary>
        /// <param name="password">AES password string</param>
        /// <param name="cypherContent">Cypher content</param>
        /// <returns>Plain content</returns>
        byte[] DecryptAES(string password, byte[] cypherContent);
    }
}
