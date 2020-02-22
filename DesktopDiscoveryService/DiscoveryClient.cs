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

        public void Start(SharedCoreModels.DiscoveryServiceInfo discoveryServiceInfo, int discoveryPort, Action<string> callbackReceived)
        {
            Task.Run(() =>
            {
                var Server = new UdpClient(discoveryPort);
                var answer = Encoding.ASCII.GetBytes(discoveryServiceInfo.ToString());

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
