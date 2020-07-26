using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DesktopService.Features.Configuration
{
    public class DistributeConfig : IDistributeConfig
    {
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly ILogger<DistributeConfig> _logger;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;
        private readonly Services.IStaticResources _staticResources;

        public DistributeConfig(ILoggerFactory loggerFactory, 
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions,
            Services.IStaticResources staticResources)
        {
            _logger = loggerFactory.CreateLogger<DistributeConfig>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _appSettings = appOptions.Value;
            _staticResources = staticResources;
        }

        public void Share()
        {
            // TODO: add shared settings for RediscoveryManager & RediscoveryManager.GUI
            string hubPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDesktopHubPath);

            HandleDiscoveryService();

            if (!string.IsNullOrWhiteSpace(hubPath) && System.IO.Directory.Exists(hubPath))
            {
                // TODO: no longer needed
                var hubConfig = new SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration
                {
                    IP = _staticResources.HostIpAddress,
                    Port = _staticResources.HostPort,
                    DesktopHubApplicationKey = _remoteResourceSettings.RediscoveryDesktopHubApplicationKey
                };
                OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.DesktopHub.ConfigFileNames.AppSettings), SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration.SectionName, hubConfig);
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
                    var discoveryFolder = Path.Combine(parentPath, "DiscoveryService");
                    if (Directory.Exists(discoveryFolder))
                        discoveryPath = discoveryFolder;
                }

                if (!string.IsNullOrWhiteSpace(discoveryPath) && System.IO.Directory.Exists(discoveryPath))
                {
                    // Update settings for discovery service
                    var serviceInfo = new SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration
                    {
                        IP = _staticResources.HostIpAddress,
                        Port = _staticResources.HostPort,
                        MetaInfo = null,
                        Name = _appSettings.DesktopName
                    };
                    OnUpdateRemoteConfiguration(Path.Combine(discoveryPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration.SectionName, serviceInfo);

                    // Add firewall rule for discovery service
                    var fwDiscovery = OnReadRemoteConfiguration<SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration>(Path.Combine(discoveryPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration.SectionName);
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
                SharedConfigurations.RemoteConfiguration.UpdateRemoteConfiguration(filePath, key, value);
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private T OnReadRemoteConfiguration<T>(string filePath, string key)
        {
            try
            {
                return SharedConfigurations.RemoteConfiguration.ReadRemoteConfiguration<T>(filePath, key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return default;
            }
        }
    }
}
