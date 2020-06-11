using CommunicationAuthenticationProvider;
using CommunicationAuthenticationProvider.Models;
using Microsoft.Extensions.Logging;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesktopService.Features.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ILogger<AuthenticationManager> _logger;
        private readonly DALDesktopService.Repository.IDevicePendingAuthenticationRepository _devicePendingAuthenticationRepository;
        private readonly DALDesktopService.Repository.IDeviceRepository _deviceRepository;
        private readonly Features.DeviceFeature.IFeatureService _featureService;

        public AuthenticationManager(ILoggerFactory loggerFactory,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            Features.DeviceFeature.IFeatureService featureService)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationManager>();
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
            _deviceRepository = deviceRepository;
            _featureService = featureService;
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
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return false;
        }

        public SharedBase.Connection.Manifest GetManifest()
        {
            // TODO: impl. Manifest creation from real data
            return new SharedBase.Connection.Manifest
            {
                AppMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                ClientVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = null },
                SupportedFeatures = _featureService.GetFeaturesManifest(),
                ClientName = "DEV-Desktop"
            };
        }

        public async Task<LoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            try
            {
                var u = await _deviceRepository.GetByDeviceIdentifier(welcomeDeviceMessage.DeviceIdentifier);
                if (u != null)
                {
                    // TODO: remove u.Token
                    u.DeviceType = welcomeDeviceMessage.DeviceType;
                    u.Idiom = welcomeDeviceMessage.Idiom;
                    u.Manufacturer = welcomeDeviceMessage.Manufacturer;
                    u.Model = welcomeDeviceMessage.Model;
                    u.OSVersion = welcomeDeviceMessage.OSVersion;
                    u.Platform = welcomeDeviceMessage.Platform;
                    u = await _deviceRepository.SaveDevice(u);
                    _logger.LogDebug($"Request login Device found (Identifier:{u.DeviceIdentifier} Name:{u.DeviceName} Allow:{u.AllowAccess})");
                    // TODO: handle get correct [Role]
                    return new LoginResult
                    {
                        Id = u.Id.ToString(),
                        Name = u.DeviceName,
                        Role = "device",
                        State = u.AllowAccess ? SharedBase.Authentication.LoginState.OK : SharedBase.Authentication.LoginState.Denied
                    };
                }
                else
                {
                    _logger.LogDebug($"Request login Device not found (Identifier:{welcomeDeviceMessage.DeviceIdentifier} Name:{welcomeDeviceMessage.DeviceName})");
                    return new LoginResult
                    {
                        Id = null,
                        Name = null,
                        Role = null,
                        State = SharedBase.Authentication.LoginState.RequiredAuthorizeKey
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return new LoginResult
            {
                Id = null,
                Name = null,
                Role = null,
                State = SharedBase.Authentication.LoginState.Failed
            };
        }
    }
}
