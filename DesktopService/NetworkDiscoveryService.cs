using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DesktopService
{
    public class NetworkDiscoveryService
    {
        public void Server()
        {
            /*EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener);
            //server.Start(9050);
            //server.SendDiscoveryRequest(Encoding.ASCII.GetBytes("DiscoveryRequest"), 9898);
            server.DiscoveryEnabled = true;
            server.Start(9898);


            listener.ConnectionRequestEvent += request =>
            {
                if (server.PeersCount < 10)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();*/

            /*var Server = new UdpClient(8888);
            var ResponseData = Encoding.ASCII.GetBytes("SomeResponseData");

            while (true)
            {
                var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                var ClientRequestData = Server.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                Console.WriteLine("Recived {0} from {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                Server.Send(ResponseData, ResponseData.Length, ClientEp);
            }*/
        }
    }
}
