using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.RemoteResources
{
    [AllowAnonymous]
    public class DiscoveryServiceRemoteResourceHub : Hub
    {
        private readonly ILogger<DesktopHubRemoteResourceHub> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;

        public DiscoveryServiceRemoteResourceHub(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings)
        {
            _logger = loggerFactory.CreateLogger<DesktopHubRemoteResourceHub>();
            _remoteResourceSettings = remoteResourceSettings.Value;
        }

        public async Task Hello(string applicationKey)
        {
            try
            {
                if (_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey == applicationKey)
                {
                    _logger.LogInformation($"DiscoveryService => Hello received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", "ok");
                } else
                {
                    _logger.LogInformation($"DiscoveryService => Hello received from unknown Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", "unknown");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
