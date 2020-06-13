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
        private readonly IRoleResolver _roleResolver;

        public AuthenticationManager(ILoggerFactory loggerFactory,
            DALDesktopService.Repository.IDevicePendingAuthenticationRepository devicePendingAuthenticationRepository,
            DALDesktopService.Repository.IDeviceRepository deviceRepository,
            Features.DeviceFeature.IFeatureService featureService,
            IRoleResolver roleResolver)
        {
            _logger = loggerFactory.CreateLogger<AuthenticationManager>();
            _devicePendingAuthenticationRepository = devicePendingAuthenticationRepository;
            _deviceRepository = deviceRepository;
            _featureService = featureService;
            _roleResolver = roleResolver;
        }

        public async Task<bool> AddPendingApprovel(GreetingDeviceMessage greetingDeviceMessage)
        {
            try
            {
                var pendingDevice = await _devicePendingAuthenticationRepository.GetByDeviceIdentifier(greetingDeviceMessage.DeviceIdentifier);
                if (pendingDevice == null)
                {
                    // if we can't find the device in pending authentication then we add it
                    pendingDevice = await _devicePendingAuthenticationRepository.SaveDevicePendingAuthentication(new DALDesktopService.Models.DevicePendingAuthentication
                    {
                        DeviceIdentifier = greetingDeviceMessage.DeviceIdentifier,
                        DeviceName = greetingDeviceMessage.DeviceName,
                        DeviceType = greetingDeviceMessage.DeviceType,
                        Idiom = greetingDeviceMessage.Idiom,
                        Manufacturer = greetingDeviceMessage.Manufacturer,
                        Model = greetingDeviceMessage.Model,
                        OSVersion = greetingDeviceMessage.OSVersion,
                        Platform = greetingDeviceMessage.Platform,
                        Id = Guid.NewGuid(),
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

        public async Task<Enums.AllowConnect> AllowedToLogin(string deviceIdentifier)
        {
            try
            {
                var u = await _deviceRepository.GetByDeviceIdentifier(deviceIdentifier);
                if (u != null)
                {
                    if (u.AllowAccess)
                    {
                        return Enums.AllowConnect.OK;
                    } else
                    {
                        return Enums.AllowConnect.Denied;
                    }
                }
                else
                {
                    return Enums.AllowConnect.UnkownDevice;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Enums.AllowConnect.Error;
            }
        }

        public string GetCertificatePEM(string deviceIdentifier)
        {
            return Program.CertPEM();
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
                    u.Role = _roleResolver.GetRole(welcomeDeviceMessage.DeviceIdentifier);
                    u = await _deviceRepository.SaveDevice(u);
                    _logger.LogDebug($"Request login Device found (Identifier:{u.DeviceIdentifier} Name:{u.DeviceName} Allow:{u.AllowAccess})");
                    return new LoginResult
                    {
                        Id = u.Id.ToString(),
                        DeviceIdentifier = u.DeviceIdentifier,
                        Role = u.Role,
                        State = u.AllowAccess ? SharedBase.Authentication.LoginState.OK : SharedBase.Authentication.LoginState.Denied
                    };
                }
                else
                {
                    _logger.LogDebug($"Request login Device not found (Identifier:{welcomeDeviceMessage.DeviceIdentifier})");
                    return new LoginResult
                    {
                        Id = null,
                        DeviceIdentifier = null,
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
                DeviceIdentifier = null,
                Role = null,
                State = SharedBase.Authentication.LoginState.Failed
            };
        }
    }
}
