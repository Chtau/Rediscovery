using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public interface IAuth
    {
        Task<Tuple<Auth.LoginState, DALDesktopService.Models.Device>> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage);
        Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage);
    }
}
