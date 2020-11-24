using Rediscovery.Communication.Authentication.Provider.Models;
using Rediscovery.Shared.Base.Authentication;
using Rediscovery.Shared.Base.Connection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Authentication.Provider
{
    public interface IAuthenticationManager
    {
        Rediscovery.Shared.Base.Connection.Manifest GetManifest();
        Task<LoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage);
        Task<bool> AddPendingApprovel(Rediscovery.Shared.Base.Connection.GreetingDeviceMessage greetingDeviceMessage);
        string GetCertificatePEM(string deviceIdentifier);
        int GetSSLPort();
        Task<Rediscovery.Shared.Base.Connection.Enums.AllowConnect> AllowedToLogin(string deviceIdentifier, GreetingDeviceMessage greetingDeviceMessage);
        bool GetSSLActive();
    }
}
