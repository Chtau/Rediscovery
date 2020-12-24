using Rediscovery.Client.App.Core.Dependency;
using Rediscovery.Client.App.Core.Features.Connect.Models;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Connect
{
    public class DevicesService : IDevicesService
    {
        private readonly ILogger _logger;
        private readonly ISettingValue<ConnectSetting> _monitorSettings;
        private List<IConnectDevice> connectDevices = new List<IConnectDevice>();

        public event EventHandler<DeviceConnectionState> ConnectionStateChanged;

        public DevicesService(ILogger logger, ISettingValue<ConnectSetting> settingValue)
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

        private IConnectDevice OnTryGetConnectDevice(Guid id)
        {
            try
            {
                var conDevice = connectDevices.FirstOrDefault(x => x.ConnectionConfiguration?.Id == id);
                if (conDevice != null)
                    return conDevice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return null;
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
                            var index = connectDevices.FindIndex(x => x.ConnectionConfiguration?.Id == configuration.Id);
                            if (index != -1)
                            {
                                connectDevices[index].SetConfiguration(configuration);
                            }
                            else
                            {
                                var newConnectDevice = new ConnectDevice(_logger, _monitorSettings);
                                newConnectDevice.SetConfiguration(configuration);
                                connectDevices.Add(newConnectDevice);
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
                            var index = connectDevices.FindIndex(x => x.ConnectionConfiguration?.Id == id);
                            if (index != -1)
                            {
                                connectDevices[index].Disconnect();
                                connectDevices[index].Dispose();
                                connectDevices.RemoveAt(index);
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
    }
}
