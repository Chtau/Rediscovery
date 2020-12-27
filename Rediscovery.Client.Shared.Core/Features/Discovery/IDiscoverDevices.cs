using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Discovery
{
    public interface IDiscoverDevices
    {
        void Start(Action<Shared.Base.Discovery.DiscoveryServiceInfo> deviceFoundCallback);
        void Stop();
    }
}
