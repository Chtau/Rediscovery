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

        public ConnectHub(Features.Authentication.IAuth auth, IManifest manifest)
        {
            _auth = auth;
            _manifest = manifest;
        }

        public async Task Welcome(string user, string identifyer)
        {
            var result = await _auth.RequestLogin(user);
            if (result == Auth.LoginState.Denied)
            {
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.Denied, null);
            }
            else if (result == Auth.LoginState.Failed)
            {
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.Error, null);
            }
            else if (result == Auth.LoginState.RequiredAuthorizeKey)
            {
                // TODO: show key on desktop
                await OnSendHello(SharedCoreModels.Enums.ConnectionState.WaitForApprovel, null);
            } else if (result == Auth.LoginState.OK)
            {
                await OnLogin(user, null);
            }
        }

        public async Task AuthorizeKey(string user, string identifyer, string key)
        {
            var result = await _auth.Authorize(user, key);
            if (result.Item1)
            {
                // TODO: if the user validate with a key we can add the user to the local db
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
