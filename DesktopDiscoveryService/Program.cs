using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDiscoveryService
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: add firewall port rule
            // netsh advfirewall firewall add rule name="Rediscovery discovery UDP Port 8888" dir=in action=allow protocol=UDP localport=8888
            // TODO: remove firewall port rule
            // netsh advfirewall firewall delete rule name="Rediscovery discovery UDP Port 8888" protocol=UDP localport=8888


            Task.Run(() =>
            {
                //var localEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.100"), 8888);
                //var Server = new UdpClient(localEndpoint);// "192.168.1.100", 8888);
                var Server = new UdpClient(8888);
                var ResponseData = Encoding.ASCII.GetBytes("SomeResponseData");

                /*UdpClient udpClient = new UdpClient();
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 8888));
                var from = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 0);*/
                while (true)
                {
                    //var recvBuffer = udpClient.Receive(ref from);
                    //Console.WriteLine(Encoding.UTF8.GetString(recvBuffer));
                    var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                    var ClientRequestData = Server.Receive(ref ClientEp);
                    var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                    Console.WriteLine("Recived {0} from {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                    //Server.Send(ResponseData, ResponseData.Length, ClientEp);*/
                }
            });
            Console.ReadKey();
        }
    }
}
