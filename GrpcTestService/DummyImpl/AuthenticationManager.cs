using CommunicationAuthenticationProvider;
using CommunicationAuthenticationProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class AuthenticationManager : IAuthenticationManager
    {
        public Task<bool> AddPendingApprovel(SharedBase.Connection.WelcomeDeviceMessage welcomeDeviceMessage)
        {
            return Task.FromResult(true);
        }

        public SharedBase.Connection.Manifest GetManifest()
        {
            return new SharedBase.Connection.Manifest
            {
                AppMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = "T" },
                ClientVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = "T" },
                ClientName = "TestService",
                SupportedFeatures = new List<SharedBase.Device.FeatureDefinitionExtended>()
            };
        }

        public Task<LoginResult> RequestLogin(SharedBase.Connection.WelcomeDeviceMessage welcomeDeviceMessage)
        {
            return Task.FromResult(new LoginResult
            {
                Id = Guid.NewGuid().ToString(),
                Name = welcomeDeviceMessage.DeviceName,
                State = SharedBase.Authentication.LoginState.OK,
                Role = welcomeDeviceMessage.DeviceIdentifier == "80" ? "resourceconsumer" : "User"
            });
        }
    }
}
