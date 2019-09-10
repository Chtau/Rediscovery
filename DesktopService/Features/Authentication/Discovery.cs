using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopService.Features.Authentication
{
    public class Discovery : IDiscovery
    {
        private readonly ILogger<Discovery> _logger;

        public Discovery(ILoggerFactory loggerFactory, IConfigurationRoot config)
        {
            _logger = loggerFactory.CreateLogger<Discovery>();
        }

        public void Open()
        {
            _logger.LogDebug("Open for discovery");
        }
    }
}
