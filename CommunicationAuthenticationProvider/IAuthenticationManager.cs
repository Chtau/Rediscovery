using Rediscovery.Communication.Provider.Authentication.Models;
using Rediscovery.Shared.Base.Connection;
using System.Threading.Tasks;

namespace Rediscovery.Communication.Provider.Authentication
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
