using Rediscovery.Client.App.Core.Features.Device;
using Rediscovery.Client.App.Core.Features.Discovery;
using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Logging;
using Rediscovery.Client.Shared.Core.Features.Storage;
using Rediscovery.Client.Shared.Core.Resources;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rediscovery.Client.App.Core
{
    public class CoreClientManagerTest
    {
        [Fact]
        public void Init()
        {
            Shared.Init();
            Assert.NotNull(Resolver.Get<ILoggingData>());
            Assert.NotNull(Resolver.Get<ILogger>());
            Assert.NotNull(Resolver.Get<IDBStorage>());
            Assert.NotNull(Resolver.Get<IJSONStorage>());
            Assert.NotNull(Resolver.Get<IAssemblyResourceProvider>());
            Assert.NotNull(Resolver.Get<IDiscoverDevices>());
            Assert.NotNull(Resolver.Get<IDevicesManager>());
        }
    }
}
