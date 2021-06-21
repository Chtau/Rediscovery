using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal static class HashExtensions
    {
        public enum HashAlgorithmTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        /// <summary>
        /// Uses <see cref="SHA512"/> as algorithm
        /// </summary>
        /// <param name="inputString">Value to Hash</param>
        /// <param name="hashAlgorithmTypes">Hash algorithm <see cref="HashAlgorithmTypes"/></param>
        /// <returns></returns>
        public static byte[] GetHash(this string inputString, HashAlgorithmTypes hashAlgorithmTypes = HashAlgorithmTypes.SHA512)
        {
            return Encoding.UTF8.GetBytes(inputString).GetHash(hashAlgorithmTypes);
        }

        /// <summary>
        /// Uses <see cref="SHA512"/> as default algorithm
        /// </summary>
        /// <param name="inputString">Value to Hash</param>
        /// <param name="hashAlgorithmTypes">Hash algorithm <see cref="HashAlgorithmTypes"/></param>
        /// <returns>Hash result as string format Hexdecimal</returns>
        public static string GetHashString(this string inputString, HashAlgorithmTypes hashAlgorithmTypes = HashAlgorithmTypes.SHA512)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in inputString.GetHash(hashAlgorithmTypes))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>
        /// Uses <see cref="SHA512"/> as algorithm
        /// </summary>
        /// <param name="input">Value to Hash</param>
        /// <param name="hashAlgorithmTypes">Hash algorithm <see cref="HashAlgorithmTypes"/></param>
        /// <returns></returns>
        public static byte[] GetHash(this byte[] input, HashAlgorithmTypes hashAlgorithmTypes = HashAlgorithmTypes.SHA512)
        {
            using (var hasher = HashAlgorithm.Create(hashAlgorithmTypes.ToString()))
                return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Uses <see cref="SHA512"/> as default algorithm
        /// </summary>
        /// <param name="input">Value to Hash</param>
        /// <param name="hashAlgorithmTypes">Hash algorithm <see cref="HashAlgorithmTypes"/></param>
        /// <returns>Hash result as string format Hexdecimal</returns>
        public static string GetHashString(this byte[] input, HashAlgorithmTypes hashAlgorithmTypes = HashAlgorithmTypes.SHA512)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in input.GetHash(hashAlgorithmTypes))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        /// <summary>
        /// Our default checksum implementation which uses the first 16 characters of <see cref="HashAlgorithmTypes.MD5"/>
        /// </summary>
        /// <param name="input">Value to get the hash for</param>
        /// <returns>Checksum for the input</returns>
        public static string GetChecksum(this byte[] input)
        {
            return input.GetHashString(HashAlgorithmTypes.MD5).Substring(0, 16);
        }

        /// <summary>
        /// Our default checksum implementation which uses the first 16 characters of <see cref="HashAlgorithmTypes.MD5"/>
        /// </summary>
        /// <param name="input">Value to get the hash for</param>
        /// <returns>Checksum for the input</returns>
        public static string GetChecksum(this string input)
        {
            return input.GetHashString(HashAlgorithmTypes.MD5).Substring(0, 16);
        }
    }
}
