using Rediscovery.Client.Shared.Core.Dependency;
using Rediscovery.Client.Shared.Core.Resources;
using Rediscovery.Client.Shared.Core.Features.Storage;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core
{
    public static class CoreManager
    {
        public static void Init(CoreManagerSetting coreManagerSettings)
        {
            Dependency.Resolver.Register<Features.Logging.ILoggingData, Features.Logging.LoggingData>();
            Dependency.Resolver.Register<ILogger, Features.Logging.EventLogger>();
            var logger = Dependency.Resolver.Get<ILogger>();
            Dependency.Resolver.Register<IDBStorage>(new DBStorage(logger, new SettingValue<StorageSetting>(coreManagerSettings.CurrentStorageSetting)));
            Dependency.Resolver.Register<IJSONStorage>(new JSONStorage(logger, new SettingValue<StorageSetting>(coreManagerSettings.CurrentStorageSetting)));
            Dependency.Resolver.Register<IAssemblyResourceProvider>(new AssemblyResourceProvider(logger));
        }
    }
}
