using Microsoft.Extensions.Logging;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    [Obsolete("no longer needed ????")]
    public class Auth : IAuth
    {
        public enum LoginState
        {
            Failed,
            Denied,
            RequiredAuthorizeKey,
            OK
        }

        private readonly ILogger<Auth> _logger;
        private readonly ITokenService _tokenService;
        private readonly DALDesktopService.Repository.IDeviceRepository _deviceRepository;
        private readonly DALDesktopService.Repository.IDevicePendingAuthenticationRepository _devicePendingAuthenticationRepository;

        public Auth(ILoggerFactory loggerFactory, ITokenService tokenService,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository)
        {
            _logger = loggerFactory.CreateLogger<Auth>();
            _tokenService = tokenService;
            _deviceRepository = deviceRepository;
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
        }

        public async Task<Tuple<LoginState, DALDesktopService.Models.Device>> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            try
            {
                var u = await _deviceRepository.GetByDeviceIdentifier(welcomeDeviceMessage.DeviceIdentifier);
                if (u != null)
                {
                    u.Token = _tokenService.CreateNewToken(u.Id.ToString(), u.DeviceName);
                    u.DeviceType = welcomeDeviceMessage.DeviceType;
                    u.Idiom = welcomeDeviceMessage.Idiom;
                    u.Manufacturer = welcomeDeviceMessage.Manufacturer;
                    u.Model = welcomeDeviceMessage.Model;
                    u.OSVersion = welcomeDeviceMessage.OSVersion;
                    u.Platform = welcomeDeviceMessage.Platform;
                    u = await _deviceRepository.SaveDevice(u);
                    _logger.LogDebug($"Request login Device found (Identifier:{u.DeviceIdentifier} Name:{u.DeviceName} Allow:{u.AllowAccess})");
                    return new Tuple<LoginState, DALDesktopService.Models.Device>(u.AllowAccess ? LoginState.OK : LoginState.Denied, u);
                }
                else
                {
                    _logger.LogDebug($"Request login Device not found (Identifier:{welcomeDeviceMessage.DeviceIdentifier} Name:{welcomeDeviceMessage.DeviceName})");
                    return new Tuple<LoginState, DALDesktopService.Models.Device>(LoginState.RequiredAuthorizeKey, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return new Tuple<LoginState, DALDesktopService.Models.Device>(LoginState.Failed, null);
        }

        public async Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            try
            {
                var pendingDevice = await _devicePendingAuthenticationRepository.GetByDeviceIdentifier(welcomeDeviceMessage.DeviceIdentifier);
                if (pendingDevice == null)
                {
                    // if we can't find the device in pending authentication then we add it
                    pendingDevice = await _devicePendingAuthenticationRepository.SaveDevicePendingAuthentication(new DALDesktopService.Models.DevicePendingAuthentication
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
                }
                _remoteResourcesSenderService.SendPendingAuthenticationDevices();
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return false;
        }
    }
}
