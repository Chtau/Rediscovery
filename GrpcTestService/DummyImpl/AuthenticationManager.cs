using CommunicationAuthenticationProvider;
using CommunicationAuthenticationProvider.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcTestService.DummyImpl
{
    public class AuthenticationManager : IAuthenticationManager
    {
        public Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            return Task.FromResult(true);
        }

        public SharedCoreModels.Manifest GetManifest()
        {
            return new SharedCoreModels.Manifest
            {
                AppMinimumVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = "T" },
                ClientVersion = new SharedBase.Core.Version() { Major = 0, Minor = 0, Patch = 0, Label = "T" },
                ClientName = "TestService",
                SupportedFeatures = new List<SharedBase.Device.FeatureDefinitionExtended>()
            };
        }

        public Task<LoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage)
        {
            return Task.FromResult(new LoginResult
            {
                Id = Guid.NewGuid().ToString(),
                Name = welcomeDeviceMessage.DeviceName,
                State = SharedBase.Authentication.LoginState.OK,
                Role = "User"
            });
        }
    }
}
