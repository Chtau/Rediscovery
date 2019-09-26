using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public interface IAuth
    {
        Task<Tuple<Auth.LoginState, Identity.Models.Device>> RequestLogin(string device);
        Task<Tuple<bool, string>> Authorize(string device, string key);
    }
}
