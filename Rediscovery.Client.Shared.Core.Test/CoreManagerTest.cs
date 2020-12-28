using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Features.Logging;
using Rediscovery.Client.Shared.Core.Features.Storage;
using Rediscovery.Client.Shared.Core.Resources;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Rediscovery.Client.Shared.Core.Test
{
    public class CoreManagerTest
    {
        [Fact]
        public void Init()
        {
            CoreManager.Init(new CoreManagerSetting
            {
                CurrentStorageSetting = new StorageSetting
                {
                    DatabaseFile = null
                }
            });
            Assert.NotNull(Resolver.Get<ILoggingData>());
            Assert.NotNull(Resolver.Get<ILogger>());
            Assert.NotNull(Resolver.Get<IDBStorage>());
            Assert.NotNull(Resolver.Get<IJSONStorage>());
            Assert.NotNull(Resolver.Get<IAssemblyResourceProvider>());
        }
    }
}
