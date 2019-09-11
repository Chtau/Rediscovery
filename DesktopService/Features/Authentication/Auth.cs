using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public class Auth : IAuth
    {
        public enum LoginState
        {
            Failed,
            Denied,
            RequiredAuthorizeKey,
            OK
        }

        public async Task<bool> Authorize(string user, string identifyer, string key)
        {
            return true;
        }

        public async Task<LoginState> RequestLogin(string user, string identifyer)
        {
            return LoginState.OK;
        }
    }
}
