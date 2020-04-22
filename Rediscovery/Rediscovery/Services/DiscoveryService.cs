using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DiscoveryService))]
namespace Rediscovery.Services
{
    public class DiscoveryService : BaseService, IDiscoveryService
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
                        // broadcast don't work in the vs android emulator due to virtual network problems... 
                        // TODO: need to add configuration for Discovery Port Setting
                        int sendBytes = Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));
                        if (sendBytes == RequestData.Length)
                        {
                            await Task.Run(() =>
                            {
                                var ServerResponseData = Client.Receive(ref ServerEp);
                                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                                //callbackAnswer?.Invoke(ServerEp.Address.ToString());
                                var serviceInfo = new SharedCoreModels.DiscoveryServiceInfo();
                                serviceInfo.Parse(ServerResponse);
                                callbackAnswer?.Invoke(serviceInfo);
                                _logger.Message($"Received {serviceInfo.ToString()} from {ServerEp.Address.ToString()}");

                                Client.Close();
                            });
                        } else
                        {
                            _logger.Message($"No valid Broadcast send (byte miss match Expected bytes:{RequestData.Length} send bytes:{sendBytes})");
                        }
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                } while (true);
            });
        }
    }
}
