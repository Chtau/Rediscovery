using Rediscovery.Client.App.Core.Features.Device;
using Rediscovery.Client.App.Core.Features.Device.Models;
using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.Shared.Core;
using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public static class CoreClientManager
    {
        public static void Init(CoreClientManagerSetting coreClientManagerSetting)
        {
            CoreManager.Init(coreClientManagerSetting);
            var logger = Resolver.Get<ILogger>();
            Resolver.Register<IDiscoverDevices>(new DiscoverDevices(logger, new SettingValue<DiscoverSetting>(coreClientManagerSetting.CurrentDiscoverSetting)));
            Resolver.Register<IDevicesManager>(new DevicesManager(logger, new SettingValue<ConnectSetting>(coreClientManagerSetting.CurrentConnectSetting)));
        }
    }
}
