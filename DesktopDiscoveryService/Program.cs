using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DesktopDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            var Server = new UdpClient(8888);
            var ResponseData = Encoding.ASCII.GetBytes("SomeResponseData");

            while (true)
            {
                //var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                //var ClientEp = new IPEndPoint(IPAddress.Broadcast, 0);
                var ClientEp = new IPEndPoint(IPAddress.Any, 8888);
                var ClientRequestData = Server.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                Console.WriteLine("Recived {0} from {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                Server.Send(ResponseData, ResponseData.Length, ClientEp);
            }
        }
    }
}
