using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal.Encryption
{
    internal static class CryptographyRandomString
    {
        public static string GetAlphanumericExtendet(int length)
        {
            string characters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789" +
                "!\"§$%&/()=?`{[]}\\+#*'~´-.,_:;^°@€";
            return Get(length, characters);
        }

        public static string GetAlphanumeric(int length)
        {
            string characters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return Get(length, characters);
        }

        public static string Get(int length, IEnumerable<char> characters)
        {
            if (length < 0 || length > int.MaxValue / 8)
                throw new ArgumentException($"Length must be between 0 and {int.MaxValue / 8}", "length");
            if (!(characters?.Count() > 0))
                throw new ArgumentException("characters must not be null or empty", "characterSet");
            var characterArray = characters.Distinct().ToArray();
            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
    }
}
