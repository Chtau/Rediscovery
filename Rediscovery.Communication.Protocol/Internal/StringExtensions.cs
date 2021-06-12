using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Removes or adds characters to the string till the exact length is reached.
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="length">Required length</param>
        /// <param name="fillCharacter">Character used to append if needed.</param>
        /// <returns>String with the desired length.</returns>
        public static string ExactLength(this string value, int length, char fillCharacter = '0')
        {
            string retValue = "";
            if (!string.IsNullOrWhiteSpace(value))
                retValue += value;
            if (retValue.Length > length)
                retValue = retValue.Substring(0, length);
            else
            {
                var dif = length - retValue.Length;
                retValue += new string(fillCharacter, dif);
            }
            return retValue;
        }
    }
}
