using DesktopService.Features.Identity;
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
        private readonly IDeviceService _deviceService;

        public DiscoveryServiceRemoteResourceHub(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IDeviceService deviceService)
        {
            _logger = loggerFactory.CreateLogger<DesktopHubRemoteResourceHub>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _deviceService = deviceService;
        }

        public async Task Hello(string applicationKey)
        {
            try
            {
                string token = _deviceService.AuthenticateRemoteResourceConsumer(applicationKey);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
                    _logger.LogInformation($"DiscoveryService => Hello received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", token);
                } else
                {
                    _logger.LogInformation($"DiscoveryService => Hello received from unknown Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
