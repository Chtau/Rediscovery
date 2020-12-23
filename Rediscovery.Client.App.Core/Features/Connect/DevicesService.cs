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
            } catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            return null;
        }

        public void AddOrUpdateConnectionConfiguration(params ConnectionConfiguration[] connectionConfigurations)
        {
            try
            {

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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }
}
