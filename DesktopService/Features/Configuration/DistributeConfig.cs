using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
            //Newtonsoft.Json.JsonConvert.DeserializeObject();
            string hubPath = System.IO.Path.GetFullPath(_pipeSettings.RediscoveryDesktopHubPath);
            string discoveryPath = System.IO.Path.GetFullPath(_pipeSettings.RediscoveryDiscoveryService);
        }
    }
}
