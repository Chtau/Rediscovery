using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Services
{
    public interface IDiscoveryService
    {
        void Boardcast(Action<SharedCoreModels.DiscoveryServiceInfo> callbackAnswer);
    }
}
