using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IDiscoveryService
    {
        void Boardcast(Action<SharedBase.Discovery.DiscoveryServiceInfo> callbackAnswer, Func<bool> interupt);
    }
}
