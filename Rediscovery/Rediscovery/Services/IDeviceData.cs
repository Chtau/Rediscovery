using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IDeviceData
    {
        string GenerateNewDeviceIDentifier();
        string GetDeviceIdentifier();
        SharedCoreModels.WelcomeDeviceMessage GetWelcomeDeviceMessage();
    }
}
