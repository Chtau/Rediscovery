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
        public static string GroupName = "desktophub";

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
        private readonly IRemoteResourcesRepository _remoteResourcesRepository;

        public DesktopHubRemoteResourceHub(ILoggerFactory loggerFactory, 
            IOptions<SharedConfigurations.DesktopService.Models.RemoteResourceConfiguration> remoteResourceSettings,
            IRemoteResourcesRepository remoteResourcesRepository)
        {
            _logger = loggerFactory.CreateLogger<DesktopHubRemoteResourceHub>();
            _remoteResourceSettings = remoteResourceSettings.Value;
            _remoteResourcesRepository = remoteResourcesRepository;
        }

        public async Task Hello(string applicationKey)
        {
            try
            {
                if (_remoteResourceSettings.RediscoveryDiscoveryServiceApplicationKey == applicationKey)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
                    _logger.LogInformation($"DesktopHub => Hello received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", "ok");
                }
                else
                {
                    _logger.LogInformation($"DesktopHub => Hello received from unknown Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", "unknown");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        public void RequestActiveDeviceInfo()
        {
            _remoteResourcesRepository.SendActiveDeviceInfo();
        }

        public void RequestDeviceInfo()
        {
            _remoteResourcesRepository.SendDeviceInfo();
        }

        public void RequestServiceFeature()
        {
            _remoteResourcesRepository.SendServiceFeature();
        }

        public void RequestDeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)
        {
            _remoteResourcesRepository.DeleteDeviceInfo(deviceInfo);
        }
    }
}
