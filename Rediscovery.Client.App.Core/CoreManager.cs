using Rediscovery.Client.App.Core.Storage;
using Rediscovery.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core
{
    public static class CoreManager
    {
        public static void Init(CoreManagerSetting coreManagerSettings)
        {
            Dependency.Resolver.Register<ILogger, Log.Logger>();
            var logger = Dependency.Resolver.Get<ILogger>();
            Dependency.Resolver.Register<IDBStorage>(new DBStorage(logger, new SettingValueStorage(coreManagerSettings.CurrentStorageSetting)));
            Dependency.Resolver.Register<IJSONStorage>(new JSONStorage(logger, new SettingValueStorage(coreManagerSettings.CurrentStorageSetting)));
        }
    }
}
