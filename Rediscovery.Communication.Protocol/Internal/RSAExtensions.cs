using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    internal static class RSAExtensions
    {
        public static string Serialize(this RSAParameters parameters)
        {
            var sw = new System.IO.StringWriter();
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, parameters);
            return sw.ToString();
        }

        public static RSAParameters DeserializeRSA(this string keyString)
        {
            var sr = new System.IO.StringReader(keyString);
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            return (RSAParameters)xs.Deserialize(sr);
        }
    }
}
