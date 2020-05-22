using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PluginFeature.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationResourceProvider
{
    [AllowAnonymous]
    public class RemoteResourceHub : Hub
    {
        private readonly ILogger<RemoteResourceHub> _logger;
        private readonly IAuthenticateService _authenticateService;
        private readonly IRemoteResourcesSenderService _remoteResourcesSenderService;
        private readonly IResourcesRepository _resourcesRepository;

        public RemoteResourceHub(ILoggerFactory loggerFactory,
            IAuthenticateService authenticateService,
            IResourcesRepository resourcesRepository,
            IRemoteResourcesSenderService remoteResourcesSenderService)
        {
            _logger = loggerFactory.CreateLogger<RemoteResourceHub>();
            _authenticateService = authenticateService;
            _remoteResourcesSenderService = remoteResourcesSenderService;
            _resourcesRepository = resourcesRepository;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupNames.Admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Hello(string applicationKey)
        {
            try
            {
                string token = _authenticateService.AuthenticationTokenRemoteResourceConsumer(applicationKey, GroupNames.Admin);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogInformation($"RemoteResourceHub => Hello received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", token);
                }
                else
                {
                    _logger.LogInformation($"RemoteResourceHub => Hello received from unknown Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("Hello", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public async Task RegisterListener(string applicationKey)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(applicationKey))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Admin);
                    _logger.LogInformation($"RemoteResourceHub => RegisterListener received from Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("RegisterListenerResponse", true);
                }
                else
                {
                    _logger.LogInformation($"RemoteResourceHub => RegisterListener received from unknown Application (Key:{applicationKey})");
                    await Clients.Caller.SendAsync("RegisterListenerResponse", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestActiveDeviceInfo()
        {
            _remoteResourcesSenderService.SendActiveDeviceInfo();
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestDeviceInfo()
        {
            _remoteResourcesSenderService.SendDeviceInfo();
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestServiceFeature()
        {
            _remoteResourcesSenderService.SendServiceFeature();
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestPendingAuthenticationDevices()
        {
            _remoteResourcesSenderService.SendPendingAuthenticationDevices();
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestDeleteDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)
        {
            try
            {
                _resourcesRepository.DeleteDeviceInfo(deviceInfo);
                _remoteResourcesSenderService.SendDeviceInfo();
                _remoteResourcesSenderService.SendActiveDeviceInfo();
                _remoteResourcesSenderService.SendPendingAuthenticationDevices();
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestUpdateDeviceInfo(SharedCoreModels.DeviceInfo deviceInfo)
        {
            try
            {
                _resourcesRepository.UpdateDeviceInfo(deviceInfo);
                _remoteResourcesSenderService.SendDeviceInfo();
                _remoteResourcesSenderService.SendActiveDeviceInfo();
                _remoteResourcesSenderService.SendPendingAuthenticationDevices();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestResolvePendingAuthenticationDevice(Guid deviceId, bool accept)
        {
            try
            {
                _resourcesRepository.ResolvePendingAuthenticationDevices(deviceId, accept);
                _remoteResourcesSenderService.SendDeviceInfo();
                _remoteResourcesSenderService.SendActiveDeviceInfo();
                _remoteResourcesSenderService.SendPendingAuthenticationDevices();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestFeatureDetails(Guid featureId)
        {
            try
            {
                _remoteResourcesSenderService.SendFeatureDetails(featureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestDeleteFeatureProfile(Guid featureId, string profileId)
        {
            try
            {
                _resourcesRepository.DeleteFeatureProfile(featureId, profileId);
                _remoteResourcesSenderService.SendServiceFeature();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestSaveFeatureProfile(Guid featureId, DeviceFeatureProfil deviceFeatureProfil)
        {
            try
            {
                _resourcesRepository.SaveFeatureProfile(featureId, deviceFeatureProfil);
                _remoteResourcesSenderService.SendFeatureDetails(featureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestSaveFeatureSettings(Guid featureId, DeviceFeatureSetting deviceFeatureSetting)
        {
            try
            {
                _resourcesRepository.SaveFeatureSettings(featureId, deviceFeatureSetting);
                _remoteResourcesSenderService.SendFeatureDetails(featureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Authorize(Roles = AuthorizationRoles.AdminRole)]
        public void RequestFeatureDetailsUI(Guid featureId)
        {
            try
            {
                _remoteResourcesSenderService.SendFeatureDetailsUI(featureId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
    }
}
