using SharedCoreModels;
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

        private readonly ITokenService _tokenService;
        private readonly DALDesktopService.Repository.IDeviceRepository _deviceRepository;
        private readonly DALDesktopService.Repository.IDevicePendingAuthenticationRepository _devicePendingAuthenticationRepository;

        public Auth(ITokenService tokenService,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository)
        {
            _tokenService = tokenService;
            _deviceRepository = deviceRepository;
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
        }

        public async Task<Tuple<LoginState, DALDesktopService.Models.Device>> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            var u = await _deviceRepository.GetByDeviceIdentifier(welcomeDeviceMessage.DeviceIdentifier);
            if (u?.AllowAccess == true)
            {
                u.Token = _tokenService.CreateNewToken(u.Id.ToString(), u.DeviceName);
                u.DeviceType = welcomeDeviceMessage.DeviceType;
                u.Idiom = welcomeDeviceMessage.Idiom;
                u.Manufacturer = welcomeDeviceMessage.Manufacturer;
                u.Model = welcomeDeviceMessage.Model;
                u.OSVersion = welcomeDeviceMessage.OSVersion;
                u.Platform = welcomeDeviceMessage.Platform;
                u = await _deviceRepository.SaveDevice(u);
                return new Tuple<LoginState, DALDesktopService.Models.Device>(LoginState.OK, u);
            }
            else
            {
                return new Tuple<LoginState, DALDesktopService.Models.Device>(LoginState.RequiredAuthorizeKey, null);
            }
        }

        public async Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            var pendingDevice = await _devicePendingAuthenticationRepository.SaveDevicePendingAuthentication(new DALDesktopService.Models.DevicePendingAuthentication
            {
                DeviceIdentifier = welcomeDeviceMessage.DeviceIdentifier,
                DeviceName = welcomeDeviceMessage.DeviceName,
                DeviceType = welcomeDeviceMessage.DeviceType,
                Id = Guid.NewGuid(),
                Idiom = welcomeDeviceMessage.Idiom,
                Manufacturer = welcomeDeviceMessage.Manufacturer,
                Model = welcomeDeviceMessage.Model,
                OSVersion = welcomeDeviceMessage.OSVersion,
                Platform = welcomeDeviceMessage.Platform,
                RequestTime = DateTime.UtcNow,
            });
            if (pendingDevice != null)
                return true;
            return false;
        }
    }
}
