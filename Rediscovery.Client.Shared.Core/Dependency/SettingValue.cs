using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Dependency
{
    public class SettingValue<TSetting> : ISettingValue<TSetting>
    {
        private TSetting setting;
        private Action<TSetting, string> listener;

        public SettingValue(TSetting setting)
        {
            this.setting = setting;
        }

        public TSetting CurrentValue => setting;

        public void Change<T>(T setting)
        {
            if (setting is TSetting storage)
            {
                this.setting = storage;
                this.listener.Invoke(this.setting, null);
            }
        }

        public TSetting Get(string name)
        {
            return setting;
        }

        public IDisposable OnChange(Action<TSetting, string> listener)
        {
            this.listener = listener;
            return null;
        }
    }
}
