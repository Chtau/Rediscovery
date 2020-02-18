using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DiscoveryService))]
namespace Rediscovery.Services
{
    public class DiscoveryService : IDiscoveryService
    {
        public void Boardcast(Action<string> callbackAnswer)
        {
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);
            Task.Run(() =>
            {
                do
                {
                    var Client = new UdpClient();
                    var ServerResponseData = Client.Receive(ref ServerEp);
                    var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                    callbackAnswer?.Invoke(ServerEp.Address?.ToString() + " Response:" + ServerResponse);

                    Client.Close();
                } while (true);
            });
            Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(1000);
                    var Client = new UdpClient();
                    var RequestData = Encoding.ASCII.GetBytes("RediscoveryClient");

                    Client.EnableBroadcast = true;
                    Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                    Client.Close();
                } while (true);
            });
        }
    }
}
