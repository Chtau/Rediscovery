using DesktopService.Features.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    // TODO: auth [Authorize]
    [AllowAnonymous]
    public class ConnectHub : Hub
    {
        private readonly Features.Authentication.IAuth _auth;
        private readonly IManifest _manifest;
        private readonly IDeviceService _deviceService;
        private readonly Pipes.IPipeIncomingConnection _pipeIncomingConnection;

        public ConnectHub(Features.Authentication.IAuth auth, IManifest manifest, IDeviceService deviceService,
            Pipes.IPipeIncomingConnection pipeIncomingConnection)
        {
            _auth = auth;
            _manifest = manifest;
            _deviceService = deviceService;
            _pipeIncomingConnection = pipeIncomingConnection;
        }
        
        public async Task Welcome(string device)
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
                var userInfo = await _deviceService.AddDevice(device);
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
                await _pipeIncomingConnection.ShowCode(userInfo.PasswordKey, userInfo.DeviceName, userInfo.PasswordKeyValidTill);
            } else if (result.Item1 == Auth.LoginState.OK)
            {
                await OnLogin(device, result.Item2.Token);
            }
        }

        public async Task AuthorizeKey(string device, string key)
        {
            var result = await _auth.Authorize(device, key);
            if (result.Item1)
            {
                await OnLogin(device, result.Item2);
            } else
            {
                var userInfo = await _deviceService.AddDevice(device);
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
                await _pipeIncomingConnection.ShowCode(userInfo.PasswordKey, userInfo.DeviceName, userInfo.PasswordKeyValidTill);
            }
        }

        private async Task OnLogin(string device, string token)
        {
            await OnSendHello(SharedCoreModels.Enums.ConnectionState.OK, token);
            await Clients.Caller.SendAsync("Manifest", _manifest.GetManifest());
        }

        private async Task OnSendHello(SharedCoreModels.Enums.ConnectionState connectionState, string token)
        {
            await Clients.Caller.SendAsync("Hello", connectionState, token);
        }
    }
}
