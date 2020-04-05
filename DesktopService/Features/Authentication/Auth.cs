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

        private readonly Features.Identity.IDeviceService _deviceService;

        public Auth(Features.Identity.IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task<Tuple<bool, string>> Authorize(string device, string key)
        {
            var u = await _deviceService.Authenticate(device, key);
            if (u != null && u.AllowAccess)
                return new Tuple<bool, string>(true, u.Token);
            else
                return new Tuple<bool, string>(false, null);
        }

        public async Task<Tuple<LoginState, Identity.Models.Device>> RequestLogin(string device)
        {
            var u = await _deviceService.GetByName(device);
            if (u != null && u.AllowAccess)
            {
                u.Token = _deviceService.CreateNewToken(u.Id.ToString(), u.DeviceName);
                return new Tuple<LoginState, Identity.Models.Device>(LoginState.OK, u);
            }
            else
                return new Tuple<LoginState, Identity.Models.Device>(LoginState.RequiredAuthorizeKey, null);
        }
    }
}
