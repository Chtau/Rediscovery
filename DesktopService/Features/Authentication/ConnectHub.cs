using DesktopService.Features.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    // TODO: auth [Authorize]
    public class ConnectHub : Hub
    {
        private readonly Features.Authentication.IAuth _auth;
        private readonly IManifest _manifest;
        private readonly IUserService _userService;

        public ConnectHub(Features.Authentication.IAuth auth, IManifest manifest, IUserService userService)
        {
            _auth = auth;
            _manifest = manifest;
            _userService = userService;
            _userService.NewUserAdded += _userService_NewUserAdded;
        }

        private void _userService_NewUserAdded(object sender, Identity.Models.User e)
        {
            // TODO: show key on desktop
        }

        public async Task Welcome(string user)
        {
            var result = await _auth.RequestLogin(user);
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
                _userService.AddUser(user);
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
            } else if (result.Item1 == Auth.LoginState.OK)
            {
                await OnLogin(user, result.Item2.Token);
            }
        }

        public async Task AuthorizeKey(string user, string key)
        {
            var result = await _auth.Authorize(user, key);
            if (result.Item1)
            {
                await OnLogin(user, result.Item2);
            } else
            {
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
            }
        }

        private async Task OnLogin(string user, string token)
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
