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
        private readonly SharedConfigurations.DesktopService.Models.PipeConfiguration _pipeSettings;
        private readonly ILogger<DistributeConfig> _logger;
        private readonly SharedConfigurations.DesktopService.Models.AppConfiguration _appSettings;

        public DistributeConfig(ILoggerFactory loggerFactory, 
            IOptions<SharedConfigurations.DesktopService.Models.PipeConfiguration> pipeSettings,
            IOptions<SharedConfigurations.DesktopService.Models.AppConfiguration> appOptions)
        {
            _logger = loggerFactory.CreateLogger<DistributeConfig>();
            _pipeSettings = pipeSettings.Value;
            _appSettings = appOptions.Value;
        }

        public void Share()
        {
            string hubPath = System.IO.Path.GetDirectoryName(_pipeSettings.RediscoveryDesktopHubPath);
            string discoveryPath = System.IO.Path.GetDirectoryName(_pipeSettings.RediscoveryDiscoveryService);
            var serviceInfo = new SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort,
                MetaInfo = null,
                Name = "Rediscovery"
            };
            OnUpdateRemoteConfiguration(Path.Combine(discoveryPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration.SectionName, serviceInfo);
            var serviceInfoHub = new SharedConfigurations.Hub.Models.ServiceInfoConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort,
            };
            OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.Hub.ConfigFileNames.AppSettings), SharedConfigurations.Hub.Models.ServiceInfoConfiguration.SectionName, serviceInfoHub);

            var fwRules = new List<SharedConfigurations.Hub.Models.FirewallRulesConfiguration>();
            var fwDiscovery = OnReadRemoteConfiguration<SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration>(Path.Combine(hubPath, SharedConfigurations.DiscoveryService.ConfigFileNames.AppSettings), SharedConfigurations.DiscoveryService.Models.DiscoveryConfiguration.SectionName);
            if (fwDiscovery != null)
            {
                fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
                {
                    ExePath = _pipeSettings.RediscoveryDiscoveryService,
                    RuleName = fwDiscovery.FirewallRuleName
                });
            }
            fwRules.Add(new SharedConfigurations.Hub.Models.FirewallRulesConfiguration
            {
                ExePath = Program.ExePath,
                RuleName = _appSettings.FirewallRuleName
            });

            OnUpdateRemoteConfiguration(Path.Combine(hubPath, SharedConfigurations.Hub.ConfigFileNames.AppSettings), SharedConfigurations.Hub.Models.FirewallRulesConfiguration.SectionName, fwRules);
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
