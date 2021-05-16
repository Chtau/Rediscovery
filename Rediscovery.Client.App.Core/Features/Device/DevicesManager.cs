using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Heartbeat.Models;
using Rediscovery.Shared.Base.Discovery;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device
{
    public class DevicesManager : IDevicesManager
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<ConnectSetting> _monitorSettings;
        private readonly List<IConnectDevice> _connectDevices = new List<IConnectDevice>();
        private System.Threading.Thread listenThread;

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;
        public event EventHandler<HeartbeatResult<ConnectionConfiguration>> HeartbeatReceived;

        public DevicesManager(ILogger logger, ISettingValue<ConnectSetting> settingValue)
        {
            _logger = logger;
            _monitorSettings = settingValue;
        }

        public void Autoconnect()
        {
            try
            {
                // TODO: add auto connect logic
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Connect(Guid connectionId)
        {
            try
            {
                OnTryGetConnectDevice(connectionId)?.Connect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public bool Disconnect(Guid connectionId)
        {
            try
            {
                return OnTryGetConnectDevice(connectionId)?.Disconnect() ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return false;
        }

        public bool Probe(Guid connectionId)
        {
            try
            {
                return OnTryGetConnectDevice(connectionId)?.Probe() ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return false;
        }

        public void AddOrUpdateConnectionConfiguration(params ConnectionConfiguration[] connectionConfigurations)
        {
            try
            {
                if (connectionConfigurations?.Count() > 0)
                {
                    foreach (var configuration in connectionConfigurations)
                    {
                        try
                        {
                            var index = _connectDevices.FindIndex(x => x.ConnectionConfiguration?.Id == configuration.Id);
                            if (index != -1)
                            {
                                _connectDevices[index].SetConfiguration(configuration);
                            }
                            else
                            {
                                var newConnectDevice = new ConnectDevice(_logger, _monitorSettings);
                                newConnectDevice.SetConfiguration(configuration);
                                // hook events
                                newConnectDevice.ConnectionStateChanged += (obj, args) => ConnectionStateChanged?.Invoke(obj, args);
                                newConnectDevice.HeartbeatReceived += (obj, args) => HeartbeatReceived?.Invoke(obj, args);
                                _connectDevices.Add(newConnectDevice);
                            }
                        } catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to set device configuration. (Id:{configuration.Id} Address:{configuration.Address} Port:{configuration.Port})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void RemoveConnectionConfiguration(params Guid[] connectionConfigurationIds)
        {
            try
            {
                if (connectionConfigurationIds?.Count() > 0)
                {
                    foreach (var id in connectionConfigurationIds)
                    {
                        try
                        {
                            var index = _connectDevices.FindIndex(x => x.ConnectionConfiguration?.Id == id);
                            if (index != -1)
                            {
                                _connectDevices[index].Disconnect();
                                _connectDevices[index].Dispose();
                                _connectDevices.RemoveAt(index);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to remove device configuration. (Id:{id})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public void Listen()//DiscoveryServiceInfo discoveryServiceInfo, int discoveryPort, Action<string> callbackReceived)
        {
            try
            {
                listenThread = new System.Threading.Thread(() =>
                {
                    try
                    {
                        //var Server = new UdpClient(discoveryPort);
                        //var answer = Encoding.ASCII.GetBytes(discoveryServiceInfo.ToString());

                        while (true)
                        {
                            /*var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                            var ClientRequestData = Server.Receive(ref ClientEp);
                            var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);
                            callbackReceived?.Invoke(ClientEp.Address.ToString());
                            Server.Send(answer, answer.Length, ClientEp);*/
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex);
                    }
                })
                {
                    Name = "DeviceListen"
                };
                listenThread.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        private IConnectDevice OnTryGetConnectDevice(Guid id)
        {
            try
            {
                var conDevice = _connectDevices.FirstOrDefault(x => x.ConnectionConfiguration?.Id == id);
                if (conDevice != null)
                    return conDevice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return null;
        }
    }
}
