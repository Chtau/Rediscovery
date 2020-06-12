using CommunicationAuthenticationProvider.Models;
using SharedBase.Authentication;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    public interface IAuthenticationManager
    {
        SharedBase.Connection.Manifest GetManifest();
        Task<LoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage);
        Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage);
        string GetCertificatePEM(string deviceIdentifier);
    }
}
