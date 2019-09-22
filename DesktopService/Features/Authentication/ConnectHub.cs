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
            var result = await _auth.RequestLogin(user, identifyer);
            if (result == Auth.LoginState.Denied)
            {
                await Clients.Caller.SendAsync("Hello", false, "Denied_Access");
            }
            else if (result == Auth.LoginState.Failed)
            {
                await Clients.Caller.SendAsync("Hello", false, "Failed_Login");
            }
            else if (result == Auth.LoginState.RequiredAuthorizeKey)
            {
                // TODO: show key on desktop
            } else if (result == Auth.LoginState.OK)
            {
                await OnLogin(user, identifyer);
            }
        }

        public async Task AuthorizeKey(string user, string identifyer, string key)
        {
            if (await _auth.Authorize(user, identifyer, key))
            {
                await OnLogin(user, identifyer);
            } else
            {
                await Clients.Caller.SendAsync("Hello", false, "Failed_Authorize");
            }
        }

        private async Task OnLogin(string user, string identifyer)
        {
            await Clients.Caller.SendAsync("Hello", SharedCoreModels.Enums.ConnectionState.OK, "");
            await Clients.Caller.SendAsync("Manifest", _manifest.GetManifest());
        }
    }
}
