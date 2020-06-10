using CommunicationAuthenticationProvider.Models;
using SharedBase.Authentication;
using SharedBase.Connection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    public interface IAuthenticationManager
    {
        SharedBase.Connection.Manifest GetManifest();
        Task<LoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage);
        Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage);
    }
}
