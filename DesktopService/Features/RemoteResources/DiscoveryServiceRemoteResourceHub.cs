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

        public async Task A(string applicationKey)
        {
            try
            {
                if (_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey == applicationKey)
                {

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
