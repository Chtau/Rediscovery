using Rediscovery.Client.Shared.Core.Features.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public static class Shared
    {
        public static void Init(int discoveryPort = 14545)
        {
            CoreClientManager.Init(new CoreClientManagerSetting
            {
                CurrentConnectSetting = new Features.Device.Models.ConnectSetting
                {
                    GreetingDeviceMessage = new Rediscovery.Shared.Base.Connection.GreetingDeviceMessage
                    {
                        DeviceIdentifier = "Test",
                        DeviceName = "Test",
                        DeviceType = "Unit",
                        Idiom = "Test",
                        Manufacturer = "None",
                        Model = "Unit",
                        OSVersion = "Unit",
                        Platform = "Unit"
                    },
                    TimeoutSeconds = 5,
                    WelcomeDeviceMessage = new Rediscovery.Shared.Base.Connection.WelcomeDeviceMessage
                    {
                        DeviceIdentifier = "Test"
                    }
                },
                CurrentDiscoverSetting = new Features.Discovery.DiscoverSetting
                {
                    Port = discoveryPort
                },
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });
        }
    }
}
