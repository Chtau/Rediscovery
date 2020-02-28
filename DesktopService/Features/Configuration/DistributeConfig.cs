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
            string hubPath = System.IO.Path.GetFullPath(_pipeSettings.RediscoveryDesktopHubPath);
            string discoveryPath = System.IO.Path.GetFullPath(_pipeSettings.RediscoveryDiscoveryService);
            var serviceInfo = new SharedConfigurations.DiscoveryService.Models.ServiceInfoConfiguration
            {
                IP = Program.HostIpAddress,
                Port = Program.HostPort,
                MetaInfo = null,
                Name = "Rediscovery"
            };
            AddOrUpdateConfiguration(discoveryPath, "ServiceInfo", serviceInfo);
            /*{
            Config:
                {
                    IsConfig: false
                }
            }*/
            //AddOrUpdateAppSetting("Config:IsConfig", true);
        }

        private void AddOrUpdateConfiguration<T>(string filePath, string key, T value)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];
                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
