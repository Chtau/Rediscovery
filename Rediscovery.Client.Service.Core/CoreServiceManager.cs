using Rediscovery.Client.Service.Core.Features.Device;
using Rediscovery.Client.Shared.Core;
using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Service.Core
{
    public static class CoreServiceManager
    {
        public static void Init(CoreServiceManagerSetting coreServiceManagerSetting)
        {
            CoreManager.Init(coreServiceManagerSetting);
            var logger = Resolver.Get<ILogger>();
            Resolver.Register<IDevicesManager>(new DevicesManager(logger));
        }
    }
}
