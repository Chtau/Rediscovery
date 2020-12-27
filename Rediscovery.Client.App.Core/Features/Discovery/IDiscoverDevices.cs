using Rediscovery.Shared.Base.Discovery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Client.App.Core.Features.Discovery
{
    public interface IDiscoverDevices
    {
        void Start(Action<DiscoveryServiceInfo> deviceFoundCallback);
        void Stop();
    }
}
