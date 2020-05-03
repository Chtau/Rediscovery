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

        public DistributeConfig(ILoggerFactory loggerFactory, 
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions)
        {
            _logger = loggerFactory.CreateLogger<DistributeConfig>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _appSettings = appOptions.Value;
        }

        public void Share()
        {
            string hubPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDesktopHubPath);
            string discoveryPath = System.IO.Path.GetDirectoryName(_remoteResourceSettings.RediscoveryDiscoveryServicePath);
            var serviceInfo = new SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort,
                MetaInfo = null,
                Name = "Rediscovery"
            };
            OnUpdateRemoteConfiguration(Path.Combine(discoveryPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration.SectionName, serviceInfo);
            /*var serviceInfoHub = new SharedConfigurations.Hub.Models.ServiceInfoConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort,
            };
            OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.Hub.ConfigFileNames.AppSettings), SharedConfigurations.Hub.Models.ServiceInfoConfiguration.SectionName, serviceInfoHub);
            */
            var fwRules = new List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration>();
            var fwDiscovery = OnReadRemoteConfiguration<SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration>(Path.Combine(discoveryPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration.SectionName);
            if (fwDiscovery != null)
            {
                fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
                {
                    ExePath = _remoteResourceSettings.RediscoveryDiscoveryServicePath,
                    RuleName = fwDiscovery.FirewallRuleName
                });
            }
            fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
            {
                ExePath = Program.ExePath,
                RuleName = _appSettings.FirewallRuleName
            });
            var hubConfig = new SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort
            };
            OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.DesktopHub.ConfigFileNames.AppSettings), SharedConfigurations.DesktopHub.Models.RemoteResourceConfiguration.SectionName, hubConfig);
            //OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.Hub.ConfigFileNames.AppSettings), SharedConfigurations.Hub.Models.FirewallRulesConfiguration.SectionName, fwRules.ToArray());
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
