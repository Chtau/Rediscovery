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

        public DistributeConfig(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.PipeConfiguration> pipeSettings)
        {
            _logger = loggerFactory.CreateLogger<DistributeConfig>();
            _pipeSettings = pipeSettings.Value;
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
            OnUpdateRemoteConfiguration(Path.Combine(discoveryPath, "appsettings.json"), "ServiceInfo", serviceInfo);
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
    }
}
