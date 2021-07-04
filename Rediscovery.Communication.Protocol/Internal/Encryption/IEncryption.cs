using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal interface IEncryption
    {
        /// <summary>
        /// Overhead for symmectic encryption signature
        /// </summary>
        int SymmetricEncryptionSignatureLength { get; }
        /// <summary>
        /// Access to the current RSA key pair
        /// </summary>
        //Keys<string> RSAKey { get; }
        /// <summary>
        /// Access to the current AES password
        /// </summary>
        string SymmetricPassword { get; }
        /// <summary>
        /// Password for the AES encryption.
        /// </summary>
        /// <param name="password">Plain text password</param>
        void SetInternSymmetric(string password);
        /// <summary>
        /// Create cypher <see cref="byte[]"/> from the raw <see cref="byte[]"/> content.
        /// </summary>
        /// <param name="password">Symmetric password string</param>
        /// <param name="content">RAW content</param>
        /// <returns>Cypher content</returns>
        byte[] EncryptSymmetric(string password, byte[] content);
        /// <summary>
        /// Creates plain <see cref="byte[]"/> from cypher content.
        /// </summary>
        /// <param name="password">Symmetric password string</param>
        /// <param name="cypherContent">Cypher content</param>
        /// <returns>Plain content</returns>
        byte[] DecryptSymmetric(string password, byte[] cypherContent);
        /// <summary>
        /// Generates a cryptographic random password based on extendet aplhanumeric
        /// </summary>
        /// <param name="length">Password length</param>
        /// <param name="seed">Seed for the secure random function</param>
        /// <returns></returns>
        string CreatePassword(int length = 64, byte[] seed = null);
    }
}
