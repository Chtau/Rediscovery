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
            Dependency.Resolver.Register<Storage.IDBStorage>(new Storage.DBStorage(coreManagerSettings.CurrentStorageSetting));
        }
    }
}
