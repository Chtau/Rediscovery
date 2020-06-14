using CommunicationAuthenticationProvider;
using CommunicationAuthenticationProvider.Models;
using SharedBase.Connection;
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

        public Task<bool> AddPendingApprovel(GreetingDeviceMessage greetingDeviceMessage)
        {
            throw new NotImplementedException();
        }

        public Task<Enums.AllowConnect> AllowedToLogin(string deviceIdentifier)
        {
            throw new NotImplementedException();
        }

        public string GetCertificatePEM(string deviceIdentifier)
        {
            throw new NotImplementedException();
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

        public int GetSSLPort()
        {
            return 5001;
        }

        public Task<LoginResult> RequestLogin(SharedBase.Connection.WelcomeDeviceMessage welcomeDeviceMessage)
        {
            return Task.FromResult(new LoginResult
            {
                Id = Guid.NewGuid().ToString(),
                State = SharedBase.Authentication.LoginState.OK,
                Role = welcomeDeviceMessage.DeviceIdentifier == "80" ? "resourceconsumer" : "device"
            });
        }
    }
}
