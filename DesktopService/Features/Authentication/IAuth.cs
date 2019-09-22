using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public interface IAuth
    {
        Task<Auth.LoginState> RequestLogin(string user, string identifyer);
        Task<bool> Authorize(string user, string identifyer, string key);
        // TODO: add generated token which can be used in signalr etc. for authentication
        //       we don't need to complete identity server but make a similar function to created identity tokens
    }
}
