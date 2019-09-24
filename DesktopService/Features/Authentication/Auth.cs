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

        private readonly Features.Identity.IUserService _userService;

        public Auth(Features.Identity.IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Tuple<bool, string>> Authorize(string user, string key)
        {
            var u = _userService.Authenticate(user, key);
            if (u != null && u.AllowAccess)
                return new Tuple<bool, string>(true, u.Token);
            else
                return new Tuple<bool, string>(false, null);
        }

        public async Task<Tuple<LoginState, Identity.Models.User>> RequestLogin(string user)
        {
            var u = _userService.GetByName(user);
            if (u != null && u.AllowAccess)
                return new Tuple<LoginState, Identity.Models.User>(LoginState.OK, u);
            else
                return new Tuple<LoginState, Identity.Models.User>(LoginState.RequiredAuthorizeKey, null);
        }
    }
}
