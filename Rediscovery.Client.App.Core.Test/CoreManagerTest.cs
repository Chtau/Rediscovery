using Rediscovery.Client.App.Core.Features.Device;
using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.App.Core.Features.Storage;
using Rediscovery.Client.App.Core.Resources;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rediscovery.Client.App.Core
{
    public class CoreManagerTest
    {
        [Fact]
        public void Init()
        {
            CoreManager.Init(new CoreManagerSetting
            {
                CurrentDiscoverSetting = new Features.Discovery.DiscoverSetting
                {
                    Port = 14545
                },
                CurrentStorageSetting = new Features.Storage.StorageSetting
                {
                    DatabaseFile = null
                }
            });
            Assert.NotNull(Dependency.Resolver.Get<ILogger>());
            Assert.NotNull(Dependency.Resolver.Get<IDBStorage>());
            Assert.NotNull(Dependency.Resolver.Get<IJSONStorage>());
            Assert.NotNull(Dependency.Resolver.Get<IAssemblyResourceProvider>());
            Assert.NotNull(Dependency.Resolver.Get<IDiscoverDevices>());
            Assert.NotNull(Dependency.Resolver.Get<IDevicesManager>());
        }
    }
}
