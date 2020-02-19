using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DesktopDiscoveryService
{
    public class DiscoveryClient
    {
        public const int Port = 8888;

        public void Start(Action<string> callbackReceived)
        {
            Task.Run(() =>
            {
                var Server = new UdpClient(Port);
                var answer = Encoding.ASCII.GetBytes($"IP:{SharedFeatureFunctions.NetworkAddress.GetIpAddr()};");

                while (true)
                {
                    var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                    var ClientRequestData = Server.Receive(ref ClientEp);
                    var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);
                    callbackReceived?.Invoke(ClientEp.Address.ToString());
                    Server.Send(answer, answer.Length, ClientEp);
                }
            });
        }
    }
}
