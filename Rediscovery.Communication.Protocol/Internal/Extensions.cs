using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal static class Extensions
    {
        /// <summary>
        /// Uses <see cref="SHA512"/> as algorithm
        /// </summary>
        /// <param name="inputString">Value to Hash</param>
        /// <returns></returns>
        public static byte[] GetHash(this string inputString)
        {
            using (HashAlgorithm algorithm = SHA512.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Uses <see cref="SHA512"/> as algorithm
        /// </summary>
        /// <param name="inputString">Value to Hash</param>
        /// <returns>Hash result as string format Hexdecimal</returns>
        public static string GetHashString(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
