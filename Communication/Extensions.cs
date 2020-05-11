using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationResourceConsumer
{
    internal static class Extensions
    {
        internal static string ToProtocolValue(this Protocol protocol)
        {
            switch (protocol)
            {
                case Protocol.HTTP:
                    return "http://";
                case Protocol.HTTPS:
                    return "https://";
                default:
                    return "http://";
            }
        }
    }
}
