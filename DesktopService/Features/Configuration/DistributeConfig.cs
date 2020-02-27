using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Configuration
{
    public class DistributeConfig : IDistributeConfig
    {
        private readonly Pipes.Models.PipeSettings _pipeSettings;
        private readonly ILogger<DistributeConfig> _logger;

        public DistributeConfig(ILoggerFactory loggerFactory, IOptions<Pipes.Models.PipeSettings> pipeSettings)
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
