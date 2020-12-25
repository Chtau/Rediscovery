using Rediscovery.Shared.Base.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Device.Models
{
    public class ConnectSetting
    {
        public GreetingDeviceMessage GreetingDeviceMessage { get; set; }
        public int TimeoutSeconds { get; set; }
        public WelcomeDeviceMessage WelcomeDeviceMessage { get; set; }
    }
}
