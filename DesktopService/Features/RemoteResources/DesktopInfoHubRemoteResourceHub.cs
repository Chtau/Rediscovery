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
    public static class ActiveDesktopInfoHandler
    {
        public static HashSet<string> ConnectionIds = new HashSet<string>();
    }

    [AllowAnonymous]
    public class DesktopInfoHubRemoteResourceHub : Hub
    {
        public static string GroupName = "infohub";

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                if (ActiveDesktopInfoHandler.ConnectionIds.Contains(Context.ConnectionId))
                    ActiveDesktopInfoHandler.ConnectionIds.Add(Context.ConnectionId);
                Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return base.OnDisconnectedAsync(exception);
        }

        private readonly ILogger<DesktopInfoHubRemoteResourceHub> _logger;
        private readonly SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration _remoteResourceSettings;
        private readonly IDeviceService _deviceService;

        public DesktopInfoHubRemoteResourceHub(ILoggerFactory loggerFactory,
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IDeviceService deviceService)
        {
            _logger = loggerFactory.CreateLogger<DesktopInfoHubRemoteResourceHub>();
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
                    if (!ActiveDesktopInfoHandler.ConnectionIds.Contains(Context.ConnectionId))
                        ActiveDesktopInfoHandler.ConnectionIds.Add(Context.ConnectionId);
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
                    _logger.LogInformation($"DesktopInfoHub => Hello received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", token);
                }
                else
                {
                    _logger.LogInformation($"DesktopInfoHub => Hello received from unknown Application (Key:{applicationKey})");
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
