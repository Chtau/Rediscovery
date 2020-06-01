using CommunicationAuthenticationProvider.Models;
using SharedCoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationAuthenticationProvider
{
    public interface IAuthenticationManager
    {
        SharedCoreModels.Manifest GetManifest();
        Task<RequestLoginResult> RequestLogin(WelcomeDeviceMessage welcomeDeviceMessage);
        Task<bool> AddPendingApprovel(WelcomeDeviceMessage welcomeDeviceMessage);
    }
}
