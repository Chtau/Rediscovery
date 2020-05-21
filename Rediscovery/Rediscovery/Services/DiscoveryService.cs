using Rediscovery.Features.Settings;
using Rediscovery.Features.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Rediscovery.Services.DiscoveryService))]
namespace Rediscovery.Services
{
    public class DiscoveryService : BaseService, IDiscoveryService
    {
        private IDataStoreGuid<SettingModel> Store => DependencyService.Get<IDataStoreGuid<SettingModel>>() ?? new SettingStore();

        public void Boardcast(Action<SharedCoreModels.DiscoveryServiceInfo> callbackAnswer, Func<bool> interupt)
        {
            Task.Run(async () =>
            {
                SettingModel setting = (await Store.GetItemsAsync()).FirstOrDefault();
                if (setting != null)
                {
                    setting = new SettingModel();
                }
                bool running = true;
                do
                {
                    try
                    {
                        running = interupt.Invoke();
                        await Task.Delay(1000);
                        var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                        var Client = new UdpClient();
                        var RequestData = Encoding.ASCII.GetBytes("RediscoveryClient");

                        Client.EnableBroadcast = true;
                        // broadcast don't work in the vs android emulator due to virtual network problems... 
                        int sendBytes = Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, setting.DiscoveryPort));
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
                        running = interupt.Invoke();
                    } catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                } while (running);
            });
        }
    }
}
