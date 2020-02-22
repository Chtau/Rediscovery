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
        public void Boardcast(Action<SharedCoreModels.DiscoveryServiceInfo> callbackAnswer)
        {
            Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        await Task.Delay(1000);
                        var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                        var Client = new UdpClient();
                        var RequestData = Encoding.ASCII.GetBytes("RediscoveryClient");

                        Client.EnableBroadcast = true;
                        Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
                        await Task.Run(() =>
                        {
                            var ServerResponseData = Client.Receive(ref ServerEp);
                            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                            //callbackAnswer?.Invoke(ServerEp.Address.ToString());
                            var serviceInfo = new SharedCoreModels.DiscoveryServiceInfo();
                            serviceInfo.Parse(ServerResponse);
                            callbackAnswer?.Invoke(serviceInfo);
                            Console.WriteLine("Recived {0} from {1}", serviceInfo.ToString(), ServerEp.Address.ToString());

                            Client.Close();
                        });
                    } catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.ToString());
                    }
                } while (true);
            });
        }
    }
}
