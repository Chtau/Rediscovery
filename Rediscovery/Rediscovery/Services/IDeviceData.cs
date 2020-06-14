using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IDeviceData
    {
        string GenerateNewDeviceIDentifier();
        string GetDeviceIdentifier();
        SharedBase.Connection.GreetingDeviceMessage GreetingDeviceMessage();
        SharedBase.Connection.WelcomeDeviceMessage WelcomeDeviceMessage();
    }
}
