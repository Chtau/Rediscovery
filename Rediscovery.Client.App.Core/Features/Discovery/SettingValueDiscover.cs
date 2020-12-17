using Rediscovery.Client.App.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Discovery
{
    public class SettingValueDiscover : ISettingValue<DiscoverSetting>
    {
        private DiscoverSetting setting;
        private Action<DiscoverSetting, string> listener;

        public SettingValueDiscover(DiscoverSetting setting)
        {
            this.setting = setting;
        }

        public DiscoverSetting CurrentValue => setting;

        public void Change<T>(T setting)
        {
            if (setting is DiscoverSetting storage)
            {
                this.setting = storage;
                this.listener.Invoke(this.setting, null);
            }
        }

        public DiscoverSetting Get(string name)
        {
            return setting;
        }

        public IDisposable OnChange(Action<DiscoverSetting, string> listener)
        {
            this.listener = listener;
            return null;
        }
    }
}
