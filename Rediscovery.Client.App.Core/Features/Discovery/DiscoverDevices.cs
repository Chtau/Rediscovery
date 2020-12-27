using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Shared.Base.Discovery;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rediscovery.Client.App.Core.Features.Discovery
{
    public class DiscoverDevices : IDiscoverDevices
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<DiscoverSetting> _monitorSettings;
        private Dictionary<DateTime, Tuple<CancellationTokenSource, Task>> listTasks = new Dictionary<DateTime, Tuple<CancellationTokenSource, Task>>();
        private bool running = false;

        public DiscoverDevices(ILogger logger, ISettingValue<DiscoverSetting> settingValue)
        {
            _logger = logger;
            _monitorSettings = settingValue;
        }

        public void Start(Action<DiscoveryServiceInfo> deviceFoundCallback)
        {
            try
            {
                running = true;
                Stop();
                Task.Run(async () =>
                {
                    var removeKeys = new List<DateTime>();
                    do
                    {
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
                                    listTasks[item].Item1.Cancel();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex);
                                }
                            }
                            removeKeys.Clear();
                        }
                        await Task.Delay(1100);
                        var token = new CancellationTokenSource();
                        listTasks.Add(DateTime.UtcNow, new Tuple<CancellationTokenSource, Task>(token, Task.Run(async () =>
                        {
                            try
                            {
                                var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                                var Client = new UdpClient();
                                var RequestData = Encoding.ASCII.GetBytes("RediscoveryClient");

                                Client.EnableBroadcast = true;
                                // broadcast don't work in the vs android emulator due to virtual network problems... 
                                int sendBytes = Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, _monitorSettings.CurrentValue.Port));
                                if (sendBytes == RequestData.Length)
                                {
                                    await Task.Run(() =>
                                    {
                                        var ServerResponseData = Client.Receive(ref ServerEp);
                                        var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                                        var serviceInfo = new DiscoveryServiceInfo();
                                        serviceInfo.Parse(ServerResponse);
                                        deviceFoundCallback?.Invoke(serviceInfo);
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
                        }, token.Token)));
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
                                listTasks[item].Item1.Cancel();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex);
                            }
                        }
                        removeKeys.Clear();
                    }
                });
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Stop()
        {
            try
            {
                running = false;
                if (listTasks.Count > 0)
                {
                    foreach (var item in listTasks)
                    {
                        item.Value.Item1.Cancel();
                    }
                }
                listTasks.Clear();
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
    }
}
