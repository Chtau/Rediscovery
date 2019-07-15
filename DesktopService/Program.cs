using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DesktopService
{
    class Program
    {
        static void Main(string[] args)
        {
            /*UdpClient listener = new UdpClient(15000);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 15000);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine($"Received broadcast from {groupEP} :");
                    Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }*/


            var listener = new UdpListener();
            listener.StartListening();
            Console.ReadLine();

            /*var net = new NetworkDiscoveryService();

            Console.WriteLine("Hello World!");
            net.Server();
            Console.ReadKey();*/
        }

        class UdpListener
        {
            //private readonly UdpClient _udpClient = new UdpClient(15000);

            public async void StartListening()
            {
                var endpoint = new IPEndPoint(IPAddress.Any, 15000);
                var _udpClient = new UdpClient(15000);
                _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpClient.EnableBroadcast = true;
                //_udpClient.JoinMulticastGroup(IPAddress.Any);
                while (true)
                {
                    var result = await _udpClient.ReceiveAsync();
                    var message = Encoding.ASCII.GetString(result.Buffer);
                    Console.WriteLine(message);
                }
            }
        }
    }
}
