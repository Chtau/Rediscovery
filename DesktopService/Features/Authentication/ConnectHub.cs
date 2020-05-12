using DesktopService.Features.FeatureDefinitions;
using DesktopService.Features.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    [AllowAnonymous]
    public class ConnectHub : Hub
    {
        private readonly ILogger<ConnectHub> _logger;
        private readonly Features.Authentication.IAuth _auth;
        private readonly IManifest _manifest;
        private readonly IDeviceService _deviceService;
        private readonly RemoteResources.IRemoteResourcesIncomingConnection _remoteResourcesIncomingConnection;

        public ConnectHub(ILoggerFactory loggerFactory, Features.Authentication.IAuth auth, IManifest manifest, IDeviceService deviceService,
            RemoteResources.IRemoteResourcesIncomingConnection remoteResourcesIncomingConnection)
        {
            _logger = loggerFactory.CreateLogger<ConnectHub>();
            _auth = auth;
            _manifest = manifest;
            _deviceService = deviceService;
            _remoteResourcesIncomingConnection = remoteResourcesIncomingConnection;
        }
        
        public async Task Welcome(string device, string deviceIdentifier)
        {
            try
            {
                var result = await _auth.RequestLogin(device);
                if (result.Item1 == Auth.LoginState.Denied)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
                }
                else if (result.Item1 == Auth.LoginState.Failed)
                {
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Error, null);
                }
                else if (result.Item1 == Auth.LoginState.RequiredAuthorizeKey)
                {
                    await _deviceService.AddPendingAuthentication(device, deviceIdentifier);
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
                }
                else if (result.Item1 == Auth.LoginState.OK)
                {
                    await OnLogin(device, result.Item2.Token);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [Obsolete("Remove after logic change")]
        public async Task AuthorizeKey(string device, string key)
        {
            try
            {
                var result = await _auth.Authorize(device, key);
                if (result.Item1)
                {
                    await OnLogin(device, result.Item2);
                }
                else
                {
                    var userInfo = await _deviceService.AddDevice(device);
                    await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
                    await _remoteResourcesIncomingConnection.ShowCode(userInfo.PasswordKey, userInfo.DeviceName, userInfo.PasswordKeyValidTill);
                }
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnLogin(string device, string token)
        {
            await OnSendHello(SharedCoreModels.Enums.ConnectionState.OK, token);
            _logger.LogInformation("Send Manifest data to the Client");
            await Clients.Caller.SendAsync("Manifest", _manifest.GetManifest());
        }

        private async Task OnSendHello(SharedCoreModels.Enums.ConnectionState connectionState, string token)
        {
            _logger.LogInformation("Send Hello information to the Client");
            await Clients.Caller.SendAsync("Hello", connectionState, token);
        }
    }
}
