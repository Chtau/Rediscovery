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
    public class DesktopHubRemoteResourceHub : Hub
    {
        private readonly ILogger<DesktopHubRemoteResourceHub> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;

        public DesktopHubRemoteResourceHub(ILoggerFactory loggerFactory, IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings)
        {
            _logger = loggerFactory.CreateLogger<DesktopHubRemoteResourceHub>();
            _remoteResourceSettings = remoteResourceSettings.Value;
        }

        public async Task A(string applicationKey)
        {
            try
            {
                if (_remoteResourceSettings.RediscoveryDesktopHubApplicationKey == applicationKey)
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
