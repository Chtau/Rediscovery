using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rediscovery.Client.App.Service.Features.Configuration
{
    public class DistributeConfig : IDistributeConfig
    {
        private readonly Shared.Configurations.Service.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly ILogger<DistributeConfig> _logger;
        private readonly Shared.Configurations.Service.Models.AppConfiguration _appSettings;
        private readonly Services.IStaticResources _staticResources;

        public DistributeConfig(ILoggerFactory loggerFactory, 
            IOptions<Shared.Configurations.Service.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IOptions<Shared.Configurations.Service.Models.AppConfiguration> appOptions,
            Services.IStaticResources staticResources)
        {
            _logger = loggerFactory.CreateLogger<DistributeConfig>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _appSettings = appOptions.Value;
            _staticResources = staticResources;
        }

        public void Share()
        {
            HandleDiscoveryService();
            HandleManager();
            HandleManagerGUI();
        }

        private void HandleManager()
        {
            try
            {
                string managerPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryManagerPath);
                if (string.IsNullOrWhiteSpace(managerPath))
                {
                    var dirInfo = Directory.GetParent(_staticResources.ExePath);
                    var parentPath = dirInfo.Parent.FullName;
                    var managerFolder = Path.Combine(parentPath, _staticResources.ManagerFolderName);
                    if (Directory.Exists(managerFolder))
                        managerPath = managerFolder;
                }

                if (!string.IsNullOrWhiteSpace(managerPath) && System.IO.Directory.Exists(managerPath))
                {
                    var config = new Shared.Configurations.Manager.Models.ConnectionConfiguration
                    {
                        IP = _staticResources.HostIpAddress,
                        Port = _staticResources.HostPort,
                        DeviceIdentifier = _remoteResourceSettings.RediscoveryManagerDeviceIdentifier,
                        AutoConnect = _remoteResourceSettings.RediscoveryManagerAutoConnect
                    };
                    OnUpdateRemoteConfiguration(Path.Combine(managerPath, Shared.Configurations.Manager.ConfigFileNames.AppSettings), Shared.Configurations.Manager.Models.ConnectionConfiguration.SectionName, config);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void HandleManagerGUI()
        {
            try
            {
                string managerPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryManagerGUIPath);
                if (string.IsNullOrWhiteSpace(managerPath))
                {
                    var dirInfo = Directory.GetParent(_staticResources.ExePath);
                    var parentPath = dirInfo.Parent.FullName;
                    var managerFolder = Path.Combine(parentPath, _staticResources.ManagerGUIFolderName);
                    if (Directory.Exists(managerFolder))
                        managerPath = managerFolder;
                }

                if (!string.IsNullOrWhiteSpace(managerPath) && System.IO.Directory.Exists(managerPath))
                {
                    var config = new Shared.Configurations.Manager.GUI.Models.ConnectionConfiguration
                    {
                        IP = _staticResources.HostIpAddress,
                        Port = _staticResources.HostPort,
                        DeviceIdentifier = _remoteResourceSettings.RediscoveryManagerDeviceIdentifier,
                        AutoConnect = _remoteResourceSettings.RediscoveryManagerAutoConnect
                    };
                    OnUpdateRemoteConfiguration(Path.Combine(managerPath, Shared.Configurations.Manager.GUI.ConfigFileNames.AppSettings), Shared.Configurations.Manager.GUI.Models.ConnectionConfiguration.SectionName, config);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void HandleDiscoveryService()
        {
            try
            {
                string discoveryPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDiscoveryServicePath);
                if (string.IsNullOrWhiteSpace(discoveryPath))
                {
                    var dirInfo = Directory.GetParent(_staticResources.ExePath);
                    var parentPath = dirInfo.Parent.FullName;
                    var discoveryFolder = Path.Combine(parentPath, _staticResources.DiscoveryServiceFolderName);
                    if (Directory.Exists(discoveryFolder))
                        discoveryPath = discoveryFolder;
                }

                if (!string.IsNullOrWhiteSpace(discoveryPath) && System.IO.Directory.Exists(discoveryPath))
                {
                    // Update settings for discovery service
                    var serviceInfo = new Shared.Configurations.Discovery.Models.ServiceInfoConfiguration
                    {
                        IP = _staticResources.HostIpAddress,
                        Port = _staticResources.HostPort,
                        MetaInfo = null,
                        Name = _appSettings.DesktopName
                    };
                    OnUpdateRemoteConfiguration(Path.Combine(discoveryPath, Shared.Configurations.Discovery.ConfigFileNames.AppSettings), Shared.Configurations.Discovery.Models.ServiceInfoConfiguration.SectionName, serviceInfo);

                    // Add firewall rule for discovery service
                    var fwDiscovery = OnReadRemoteConfiguration<Shared.Configurations.Discovery.Models.DiscoveryConfiguration>(Path.Combine(discoveryPath, Shared.Configurations.Discovery.ConfigFileNames.AppSettings), Shared.Configurations.Discovery.Models.DiscoveryConfiguration.SectionName);
                    if (fwDiscovery != null)
                    {

                    }
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void OnUpdateRemoteConfiguration<T>(string filePath, string key, T value)
        {
            try
            {
                Shared.Configurations.RemoteConfiguration.UpdateRemoteConfiguration(filePath, key, value);
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private T OnReadRemoteConfiguration<T>(string filePath, string key)
        {
            try
            {
                return Shared.Configurations.RemoteConfiguration.ReadRemoteConfiguration<T>(filePath, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return default;
            }
        }
    }
}
