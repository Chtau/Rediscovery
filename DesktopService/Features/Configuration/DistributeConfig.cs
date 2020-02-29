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
            UpdateRemoteConfiguration(Path.Combine(discoveryPath, "appsettings.json"), "ServiceInfo", serviceInfo);
            /*{
            Config:
                {
                    IsConfig: false
                }
            }*/
            //AddOrUpdateAppSetting("Config:IsConfig", true);
        }

        private void UpdateRemoteConfiguration<T>(string filePath, string key, T value)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                List<string> sections = new List<string>();
                if (key.Contains(":"))
                {
                    sections.AddRange(key.Split(":"));
                } else
                {
                    sections.Add(key);
                }
                if (sections.Count > 0)
                {
                    dynamic obj = jsonObj[sections[0]];
                    for (int i = 1; i < sections.Count; i++)
                    {
                        obj = obj[sections[i]];
                    }
                    
                    var section = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj.ToString());
                    if (section != null)
                        section = value;
                }
                else
                {
                    //jsonObj[sectionPath] = value; // if no sectionpath just set the value
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
