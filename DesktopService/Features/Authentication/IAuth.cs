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
    }
}
