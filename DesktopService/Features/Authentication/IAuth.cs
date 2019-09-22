using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public interface IAuth
    {
        Task<Auth.LoginState> RequestLogin(string user);
        Task<Tuple<bool, string>> Authorize(string user, string key);
    }
}
