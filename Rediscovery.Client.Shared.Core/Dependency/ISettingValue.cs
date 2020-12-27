using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.Shared.Core.Dependency
{
    public interface ISettingValue<out TSetting>
    {
        TSetting CurrentValue { get; }

        TSetting Get(string name);
        IDisposable OnChange(Action<TSetting, string> listener);
        void Change<T>(T setting);
    }
}
