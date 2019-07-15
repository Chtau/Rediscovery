using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Rediscovery.Services
{
    public class NetworkDiscovery : INetworkDiscovery
    {
        public void Send()
        {
            /*EventBasedNetListener listener = new EventBasedNetListener();
            NetManager client = new NetManager(listener);
            client.SendDiscoveryRequest(Encoding.ASCII.GetBytes("DiscoveryRequest"), 9898);
            client.Start(9898);
            //client.Start(9898);
            //client.Connect("localhost" , 9050 , "SomeConnectionKey" );
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                var content = "We got: " + dataReader.GetString(100);
                System.Diagnostics.Debug.Print(content);
                Console.WriteLine(content);
                dataReader.Recycle();
            };

            //while (!Console.KeyAvailable)
            while (true)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }

            client.Stop();*/

            /*var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);

            Client.EnableBroadcast = true;
            Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

            var ServerResponseData = Client.Receive(ref ServerEp);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            Console.WriteLine("Recived {0} from {1}", ServerResponse, ServerEp.Address.ToString());

            Client.Close();*/

            using (var client = new UdpClient())
            {
                client.EnableBroadcast = true;
                //var endpoint = new IPEndPoint(IPAddress.Broadcast, 15000);
                //var endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.160"), 15000); // works with correct ip
                //var endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.0"), 15000);
                var endpoint = new IPEndPoint(IPAddress.Any, 15000);
                var message = Encoding.ASCII.GetBytes("Hello World - " + DateTime.Now.ToString());
                client.Send(message, message.Length, endpoint);
                client.Close();
            }
        }
    }
}
