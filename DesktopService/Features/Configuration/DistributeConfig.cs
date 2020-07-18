using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            string hubPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDesktopHubPath);
            string discoveryPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDiscoveryServicePath);

            // TODO: we do nothing here with the firewall rules
            var fwRules = new List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration>();

            fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
            {
                ExePath = _staticResources.ExePath,
                RuleName = _appSettings.FirewallRuleName
            });

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
                    fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
                    {
                        ExePath = _remoteResourceSettings.RediscoveryDiscoveryServicePath,
                        RuleName = fwDiscovery.FirewallRuleName
                    });
                }
            }

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
