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
        public static string GroupName = "discoveryservice";

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return base.OnDisconnectedAsync(exception);
        }

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
                if (_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey.Equals(applicationKey, StringComparison.OrdinalIgnoreCase))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
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
