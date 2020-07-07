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

        public void Boardcast(Action<SharedBase.Discovery.DiscoveryServiceInfo> callbackAnswer, Func<bool> interupt)
        {
            Task.Run(async () =>
            {
                var listTasks = new Dictionary<DateTime, Task>();
                var removeKeys = new List<DateTime>();
                SettingModel setting = (await Store.GetItemsAsync()).FirstOrDefault();
                if (setting == null)
                {
                    setting = new SettingModel();
                }
                bool running = true;
                do
                {
                    running = interupt.Invoke();
                    foreach (var item in listTasks)
                    {
                        if (DateTime.UtcNow - item.Key > TimeSpan.FromSeconds(1))
                            removeKeys.Add(item.Key);
                    }
                    if (removeKeys.Count > 0)
                    {
                        foreach (var item in removeKeys)
                        {
                            try
                            {
                                listTasks[item].Dispose();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex);
                            }
                        }
                        removeKeys.Clear();
                    }
                    await Task.Delay(1100);
                    running = interupt.Invoke();
                    listTasks.Add(DateTime.UtcNow, Task.Run(async () =>
                    {
                        try
                        {
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
                                    var serviceInfo = new SharedBase.Discovery.DiscoveryServiceInfo();
                                    serviceInfo.Parse(ServerResponse);
                                    callbackAnswer?.Invoke(serviceInfo);
                                    _logger.LogTrace($"Received {serviceInfo} from {ServerEp.Address}");

                                    Client.Close();
                                });
                            }
                            else
                            {
                                _logger.LogInformation($"No valid Broadcast send (byte miss match Expected bytes:{RequestData.Length} send bytes:{sendBytes})");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                        }
                    }));
                } while (running);
                foreach (var item in listTasks)
                {
                    removeKeys.Add(item.Key);
                }
                if (removeKeys.Count > 0)
                {
                    foreach (var item in removeKeys)
                    {
                        try
                        {
                            listTasks[item].Dispose();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex);
                        }
                    }
                    removeKeys.Clear();
                }
            });
        }
    }
}
