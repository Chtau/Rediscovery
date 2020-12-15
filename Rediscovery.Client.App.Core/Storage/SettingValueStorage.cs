using Rediscovery.Client.App.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Storage
{
    public class SettingValueStorage : ISettingValue<StorageSetting>
    {
        private StorageSetting storageSetting;
        private Action<StorageSetting, string> listener;

        public SettingValueStorage(StorageSetting storageSetting)
        {
            this.storageSetting = storageSetting;
        }

        public StorageSetting CurrentValue => storageSetting;

        public void Change<T>(T setting)
        {
            if (setting is StorageSetting storage)
            {
                storageSetting = storage;
                this.listener.Invoke(storageSetting, null);
            }
        }

        public StorageSetting Get(string name)
        {
            return storageSetting;
        }

        public IDisposable OnChange(Action<StorageSetting, string> listener)
        {
            this.listener = listener;
            return null;
        }
    }
}
