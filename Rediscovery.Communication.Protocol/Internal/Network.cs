using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Communication.Protocol.Internal
{
    public static class Network
    {
        internal const string EOF = "!#~^%$|";
        internal static readonly byte[] EOFBytes = Encoding.UTF8.GetBytes(Network.EOF);

        internal static Socket CreateSocket(int port, SocketType type = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var endpoint = new IPEndPoint(ipHostInfo.AddressList[0], port);
            return new Socket(endpoint.AddressFamily, type, protocolType);
        }

        internal static EndPoint LocalEndPoint(int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            return new IPEndPoint(ipHostInfo.AddressList[0], port);
        }
    }
}
